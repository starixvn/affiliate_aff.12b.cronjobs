using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Repositories;
using Fanex.Logging;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Text;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public class ImportDataService(
        ILogger logger,
        IQueueRepository queueRepository,
        ITrackerRepository trackerRepository,
        IPullDataService pullDataService) : IImportDataService
    {
        private static readonly string SERVER_NAME = Dns.GetHostName();

        public async Task<DateTime> GetTimeTrackerAsync(int tpType, string subItem = DefaultValue.String)
        {
            var timeTracker = await trackerRepository.GetTracker(tpType, subItem);

            if (timeTracker is null)
            {
                var now = DateTimeOffset.Now;
                return new DateTimeOffset(now.Year, now.Month, 01, 00, 00, 00, TimeSpan.Zero).DateTime;
            }

            return (DateTime)timeTracker;
        }

        public async Task UpdateTimeTrackerAsync(TrackerParam param)
        {
            await trackerRepository.AddTracker(param);

            var message = $"{SERVER_NAME} - Updated {Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)}'s Tracker Time to {param.CallTime.FormatDateTime()}";
            logger.Info(message);
        }

        public async Task<string> ImportDataToDbAsync<T>(ThirdPartyApiParam param, bool isFromRunningJob = false) where T : Ticket
        {
            var logBuilder = new StringBuilder();
            var message = $"import {Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} from {param.BeginDate.FormatDateTime()} to {param.EndDate.FormatDateTime()}";

            try
            {
                logger.Info($"{SERVER_NAME} - Starting {message}");

                var pullRange = TimeSpan.FromMinutes(param.MaxMinutesRange);
                var timeRanges = DateTimeHelper.GetProportionTimeRange(param.BeginDate, param.EndDate, pullRange);

                foreach (var range in timeRanges)
                {

                    param.BeginDate = range.Item1;
                    param.EndDate = range.Item2;

                    await Task.Delay(param.MillisecondsDeplay, CancellationToken.None);
                    await ImportDataByTimeRangeToDbAsync<T>(param, isFromRunningJob).ConfigureAwait(false);
                }

                logger.Info($"{SERVER_NAME} - Finished {message}");

                return $"Finished {message}";
            }
            catch (Exception ex)
            {
                logBuilder.AppendLine($"{SERVER_NAME} - {ErrorMessageHelper.GenerateMessage(ex, null, $"Range {message}")}");

                var logMessage = logBuilder.ToString();

                await logger.ErrorAsync(logMessage);

                return logMessage;
            }
        }

        private async Task ImportDataByTimeRangeToDbAsync<T>(ThirdPartyApiParam param, bool isFromRunningJob) where T : Ticket
        {
            var dataList = await pullDataService.GetJsonDataListAsync<T>(param);
            var queueParam = new QueueParam
            {
                TpType = param.TpType,
                SubItem = param.SubItem
            };

            foreach (var data in dataList)
            {
                queueParam.JsonData = HandleSerializeData(data, param);

                await queueRepository.AddQueue(queueParam);
            }

            if (isFromRunningJob)
            {
                var trackerParam = new TrackerParam
                {
                    TpType = param.TpType,
                    SubItem = param.SubItem,
                    CallTime = param.EndDate
                };
                await trackerRepository.AddTracker(trackerParam);
            }
            else
            {
                var productName = Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem);
                logger.Info($"{SERVER_NAME} - Finished manually import {productName} from {param.BeginDate.FormatDateTime()} to {param.EndDate.FormatDateTime()}");
            }
        }

        private static string HandleSerializeData<T>(ThirdPartyProductJson<T> data, ThirdPartyApiParam param) where T : Ticket
        {
            var ticket = data.BetTransTicket.FirstOrDefault();
            var typeOfTicket = ticket?.GetType();

            if (typeOfTicket is null || typeOfTicket.Name == typeof(Ticket).Name)
            {
                return JsonConvert.SerializeObject(data);
            }

            if (typeOfTicket.Name == typeof(ConvertedTransactionIdTicket).Name)
            {
                var newData = data as ThirdPartyProductJson<ConvertedTransactionIdTicket>;

                var newBetTransTicket = newData.BetTransTicket.Select(t =>
                {
                    var uniqueTransactionId = t.TransactiondateTime + t.TransactionId;
                    t.TransactionId = uniqueTransactionId;

                    return t;
                }).ToList();

                newData.BetTransTicket = newBetTransTicket;

                return JsonConvert.SerializeObject(newData);
            }

            if (typeOfTicket.Name == typeof(AdjustedDateTimeTicket).Name)
            {
                try
                {
                    var newData = data as ThirdPartyProductJson<AdjustedDateTimeTicket>;

                    var newBetTransTicket = newData.BetTransTicket.Select(t =>
                    {
                        var transactiondateTime = DateTime.ParseExact($"{t.TransactiondateTime}", param.FormatDateTime, CultureInfo.InvariantCulture);
                        var convertedTransactiondateTime = transactiondateTime.AddHours(param.HoursAdjustment);
                        t.TransactiondateTime = convertedTransactiondateTime.ToString(param.FormatDateTime);

                        return t;
                    }).ToList();

                    newData.BetTransTicket = newBetTransTicket;

                    return JsonConvert.SerializeObject(newData);
                }
                catch (Exception ex)
                {
                    var productName = Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem);
                    var message = $"{SERVER_NAME} - {productName} - Can't ParseExact DateTime with {param.FormatDateTime} format: {ex.Message}";

                    throw ErrorMessageHelper.FormatException(message, param);
                }
            }

            return JsonConvert.SerializeObject(data);
        }
    }
}
