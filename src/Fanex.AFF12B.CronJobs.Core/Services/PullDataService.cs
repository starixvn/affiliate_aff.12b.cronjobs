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
        private const int MAX_TICKETS_IN_RANGE = 2000;

        private const string CODE_SUCCESS = "GT_S_00";
        private const string COUNT_TOKEN = "$.Count";
        private const string SYSTEM_CODE_TOKEN = "$.SystemCode";
        private const string SYSTEM_MESSAGE_TOKEN = "$.SystemCodeMessage";
        private const string BET_TRANS_TICKET_TOKEN = "$.BetTransTicket";

        public PullDataService(
            IThirdPartyProductApi thirdPartyProductApi)
        {
            _thirdPartyProductApi = thirdPartyProductApi;
        }

        public async Task<IEnumerable<string>> GetRawDataListAsync(ThirdPartyApiParam param)
        {
            var rawDataList = new List<string>();
            var message = $"Finished get {Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)}(TpType: '{param.TpType}', SubItem: '{param.SubItem}') from {param.BeginDate.FormatDateTime()} to {param.EndDate.FormatDateTime()}";
            var pullRange = TimeSpan.FromMinutes(param.MaxMinutesRange);
            var timeRanges = DateTimeHelper.GetProportionTimeRange(param.BeginDate, param.EndDate, pullRange);

            foreach (var range in timeRanges)
            {
                param.BeginDate = range.Item1;
                param.EndDate = range.Item2;

                var rawData = await GetRawDataAsync(param);

                rawDataList.Add(rawData);
            }

            Logger.Log.Info(message);

            return rawDataList;
        }

        public async Task<IEnumerable<JObject>> GetUnescapedDataByTimeRangeAsync(ThirdPartyApiParam param)
        {
            var rawData = await GetRawDataAsync(param);
            return GetUnescapedAndRemovedDataInfoByTicketRange(rawData, param);
        }

        public async Task<IEnumerable<ThirdPartyProductInfo>> GetJsonDataListAsync(ThirdPartyApiParam param)
        {
            var rawDataList = await GetRawDataListAsync(param);
            var convertedDataList = new List<ThirdPartyProductInfo>();

            foreach (var rawData in rawDataList)
            {
                convertedDataList.AddRange(GetConvertedDataList(rawData.ToString(), param));
            }

            return convertedDataList;
        }

        private async Task<string> GetRawDataAsync(ThirdPartyApiParam param)
        {
            return await _thirdPartyProductApi.GetTPBettingTickets(param);
        }

        public static IEnumerable<JObject> GetUnescapedAndRemovedDataInfoByTicketRange(string rawData, ThirdPartyApiParam param = null)
        {
            var dataJObject = JObject.Parse(rawData);
            var systemCodeToken = dataJObject.SelectToken(SYSTEM_CODE_TOKEN).ToString();

            if (string.IsNullOrEmpty(systemCodeToken) || !systemCodeToken.Contains(CODE_SUCCESS))
            {
                var errorMessage = dataJObject.SelectToken(SYSTEM_MESSAGE_TOKEN).ToString();
                var message = $"The Api return error code {systemCodeToken}, error message{errorMessage}: TpType={param.TpType}, SubItem={param.SubItem} and from {param.BeginDate.FormatDateTime()} to {param.EndDate.FormatDateTime()}";
                Logger.Log.Error(message);

                throw new Exception($"The Api return error code {systemCodeToken}, error message{errorMessage}");
            }

            var betTransTicketJToken = dataJObject.SelectToken(BET_TRANS_TICKET_TOKEN).ToString();

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

            return GetUnescapedDataByTicketRange(rawData, ticketList);
        }

        private static IEnumerable<JObject> GetUnescapedDataByTicketRange(string rawData, JArray betTransTicketList)
        {
            var rangeNumbers = betTransTicketList.Count / MAX_TICKETS_IN_RANGE;
            var unescapedDataByRangeList = new List<JObject>();

            if (rangeNumbers < MIN_RANGE_NUMBER)
            {
                var dataJObject = JObject.Parse(rawData);
                dataJObject.SelectToken(BET_TRANS_TICKET_TOKEN).Replace(betTransTicketList);
                unescapedDataByRangeList.Add(dataJObject);
            }
            else
            {
                var betTransTicketChunks = betTransTicketList.Chunk(MAX_TICKETS_IN_RANGE);

                var chunkNumber = betTransTicketChunks.Count();
                foreach (var betTransTicket in betTransTicketChunks)
                {
                    chunkNumber--;
                    var dataJObject = JObject.Parse(rawData);
                    var ticketList = new JArray(betTransTicket);
                    dataJObject.SelectToken(BET_TRANS_TICKET_TOKEN).Replace(ticketList);

                    var ticketNumber = chunkNumber == 0 ? ticketList.Count : MAX_TICKETS_IN_RANGE;
                    dataJObject.SelectToken(COUNT_TOKEN).Replace(ticketNumber);

                    unescapedDataByRangeList.Add(dataJObject);
                }
            }

            return unescapedDataByRangeList;
        }

        private static IEnumerable<ThirdPartyProductInfo> GetConvertedDataList(string rawData, ThirdPartyApiParam param)
        {
            try
            {
                var dataRanges = GetUnescapedAndRemovedDataInfoByTicketRange(rawData, param);

                return ConvertData(dataRanges);
            }
            catch (Exception ex)
            {
                var logMessage = ErrorMessageHelper.GenerateMessage(ex);
                Logger.Log.Error(logMessage);

                throw;
            }
        }

        private static IEnumerable<ThirdPartyProductInfo> ConvertData(IEnumerable<JObject> dataList)
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
