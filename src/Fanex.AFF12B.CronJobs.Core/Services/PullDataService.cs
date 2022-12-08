using Fanex.AFF12B.CronJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.Core.Contants;
using Fanex.AFF12B.CronJobs.Core.Models.DTO;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Fanex.Logging;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public class PullDataService : IPullDataService
    {
        private readonly IThirdPartyProductApi _thirdPartyProductApi;

        private const int MIN_RANGE_NUMBER = 1;
        private const int MAX_ITEMS_IN_RANGE = 2000;

        private const string BetTransTicketToken = "$.BetTransTicket";
        private const string CountToken = "$.Count";

        public PullDataService(
            IThirdPartyProductApi thirdPartyProductApi)
        {
            _thirdPartyProductApi = thirdPartyProductApi;
        }

        public async Task<IEnumerable<string>> GetRawDataListAsync(ThirdPartyApiParam param)
        {
            var rawDataList = new List<string>();
            var message = $"get data for: TpType={param.TpType}, SubItem={param.SubItem} and from {param.BeginDate.ToString(Format.DateTimeFormat)} to {param.EndDate.ToString(Format.DateTimeFormat)}";
            var pullRange = TimeSpan.FromMinutes(param.MaxMinutesRange);
            var timeRanges = DateTimeHelper.GetProportionTimeRange(param.BeginDate, param.EndDate, pullRange);

            Logger.Log.Info($"Start {message}");

            foreach (var range in timeRanges)
            {
                param.BeginDate = range.Item1;
                param.EndDate = range.Item2;

                var rawData = await GetRawDataAsync(param);

                rawDataList.Add(rawData);
            }

            Logger.Log.Info($"Finished {message}");

            return rawDataList;
        }

        public async Task<IEnumerable<JObject>> GetUnescapedAndRemoveDataInfoDataByTimeRangeAsync(ThirdPartyApiParam param)
        {
            try
            {
                var rawData = await GetRawDataAsync(param);
                return GetUnescapedAndRemoveDataInfoDataList(rawData);
            }
            catch (Exception ex)
            {
                var message = $"get data for: TpType={param.TpType}, SubItem={param.SubItem} and from {param.BeginDate.ToString(Format.DateTimeFormat)} to {param.EndDate.ToString(Format.DateTimeFormat)}";
                var logBuilder = new StringBuilder();
                logBuilder.AppendLine($"Throw exception when {message}");
                logBuilder.AppendLine(ErrorMessageHelper.GenerateMessage(ex));

                var logMessage = logBuilder.ToString();

                Logger.Log.Error(logMessage);

                throw;
            }
        }

        public async Task<IEnumerable<ThirdPartyProductInfo>> GetJsonDataListAsync(ThirdPartyApiParam param)
        {
            var rawDataList = await GetRawDataListAsync(param);
            var convertedDataList = new List<ThirdPartyProductInfo>();

            foreach (var rawData in rawDataList)
            {
                convertedDataList.AddRange(GetConvertedDataList(rawData.ToString()));
            }

            return convertedDataList;
        }

        private async Task<string> GetRawDataAsync(ThirdPartyApiParam param)
        {
            return await _thirdPartyProductApi.GetTPBettingTickets(param);
        }

        public static IEnumerable<JObject> GetUnescapedAndRemoveDataInfoDataList(string rawData)
        {
            var dataJObject = JObject.Parse(rawData);
            var betTransTicketJToken = dataJObject.SelectToken(BetTransTicketToken).ToString();

            if (dataJObject is null || string.IsNullOrEmpty(betTransTicketJToken))
            {
                return new List<JObject> { dataJObject };
            }

            var betTransTicketList = JArray.Parse(betTransTicketJToken);

            var ticketList = new JArray();
            foreach (var item in betTransTicketList)
            {
                var ticket = JObject.Parse(item.ToString());

                ticket.Remove("datainfo");

                ticketList.Add(ticket);
            }

            return GetUnescapedDataByRange(dataJObject, ticketList);
        }

        private static IEnumerable<JObject> GetUnescapedDataByRange(JObject dataJObject, JArray betTransTicketList)
        {
            var rangeNumbers = betTransTicketList.Count / MAX_ITEMS_IN_RANGE;
            var unescapedDataByRangeList = new List<JObject>();

            if (rangeNumbers < MIN_RANGE_NUMBER)
            {
                dataJObject.SelectToken(BetTransTicketToken).Replace(betTransTicketList);
                unescapedDataByRangeList.Add(dataJObject);
            }
            else
            {
                var betTransTicketChunks = betTransTicketList.Chunk(MAX_ITEMS_IN_RANGE);
                foreach (var betTransTicket in betTransTicketChunks)
                {
                    var ticketList = new JArray(betTransTicket);
                    dataJObject.SelectToken(BetTransTicketToken).Replace(ticketList);
                    dataJObject.SelectToken(CountToken).Replace(ticketList.Count);

                    unescapedDataByRangeList.Add(dataJObject);
                }
            }

            return unescapedDataByRangeList;
        }

        private IEnumerable<ThirdPartyProductInfo> GetConvertedDataList(string rawData)
        {
            try
            {
                var dataList = GetUnescapedAndRemoveDataInfoDataList(rawData);

                return GetConvertedDataByRange(dataList);
            }
            catch (Exception ex)
            {

                var logMessage = ErrorMessageHelper.GenerateMessage(ex);
                Logger.Log.Error(logMessage);

                throw;
            }
        }

        private static IEnumerable<ThirdPartyProductInfo> GetConvertedDataByRange(IEnumerable<JObject> dataList)
        {
            var convertedDataByRangeList = new List<ThirdPartyProductInfo>();

            foreach (var data in dataList)
            {
                convertedDataByRangeList.Add(JsonConvert.DeserializeObject<ThirdPartyProductInfo>(data.ToString()));
            }

            return convertedDataByRangeList;
        }
    }
}
