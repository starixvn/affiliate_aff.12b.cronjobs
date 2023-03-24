using Fanex.AFF12B.CronJobs.Core.Contants;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Repositories;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Newtonsoft.Json;
using System.Text;
using Fanex.AFF12B.CronJobs.Core.Constant;
using Fanex.Logging;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Threading;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public class ImportDataService : IImportDataService
    {
        private static readonly DateTime START_TIME_DEFAULT = new DateTime(2023, 03, 01, 00, 00, 00, 00);

        private readonly IQueueRepository _queueRepository;
        private readonly ITrackerRepository _trackerRepository;
        private readonly IPullDataService _pullDataService;

        public ImportDataService(
            IQueueRepository queueRepository,
            ITrackerRepository trackerRepository,
            IPullDataService pullDataService)
        {
            _queueRepository = queueRepository;
            _trackerRepository = trackerRepository;
            _pullDataService = pullDataService;
        }

        public async Task<DateTime> GetTimeTrackerAsync(int tpType, string subItem = DefaultValue.String)
        {
            var timeTracker = await _trackerRepository.GetTracker(tpType, subItem);
            return timeTracker is null ? START_TIME_DEFAULT : (DateTime)timeTracker;
        }

        public async Task UpdateTimeTrackerAsync(TrackerParam param)
        {
            await _trackerRepository.AddTracker(param);

            var message = $"Updated {Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)}'s Tracker Time to {param.CallTime.FormatDateTime()}";
            Logger.Log.Info(message);
        }

        public async Task<string> ImportDataToDbAsync(ThirdPartyApiParam param, bool addTrackerTime = false)
        {
            var logBuilder = new StringBuilder();
            var message = $"import {Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} from {param.BeginDate.FormatDateTime()} to {param.EndDate.FormatDateTime()}";

            try
            {
                var pullRange = TimeSpan.FromMinutes(param.MaxMinutesRange);
                var timeRanges = DateTimeHelper.GetProportionTimeRange(param.BeginDate, param.EndDate, pullRange);

                foreach (var range in timeRanges)
                {

                    param.BeginDate = range.Item1;
                    param.EndDate = range.Item2;

                    await Task.Delay(param.MillisecondsDeplay, CancellationToken.None);
                    await ImportDataByTimeRangeToDbAsync(param, addTrackerTime).ConfigureAwait(false);
                }

                return $"Finished {message}";
            }
            catch (Exception ex)
            {
                logBuilder.AppendLine($"Throw exception when {message}");
                logBuilder.AppendLine(ErrorMessageHelper.GenerateMessage(ex));

                var logMessage = logBuilder.ToString();

                await Logger.Log.ErrorAsync(logMessage);

                return logMessage;
            }
        }

        private async Task ImportDataByTimeRangeToDbAsync(ThirdPartyApiParam param, bool addTrackerTime)
        {
            var dataList = await _pullDataService.GetUnescapedDataByTimeRangeAsync(param);
            var queueParam = new QueueParam
            {
                TpType = param.TpType,
                SubItem = param.SubItem
            };

            foreach (var data in dataList)
            {
                queueParam.JsonData = JsonConvert.SerializeObject(data);

                await _queueRepository.AddQueue(queueParam);
            }

            if(addTrackerTime)
            {
                var trackerParam = new TrackerParam
                {
                    TpType = param.TpType,
                    SubItem = param.SubItem,
                    CallTime = param.EndDate
                };
                await _trackerRepository.AddTracker(trackerParam);
            }
        }
    }
}
