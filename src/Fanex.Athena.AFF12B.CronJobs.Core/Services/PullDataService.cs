using Fanex.Athena.AFF12B.CronJobs.Core.Apis;
using Fanex.Athena.AFF12B.CronJobs.Core.Configurations;
using Fanex.Athena.AFF12B.CronJobs.Core.Contants;
using Fanex.Athena.AFF12B.CronJobs.Core.Models.DTO;
using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.Athena.AFF12B.CronJobs.Core.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Services
{
    public class PullDataService : IPullDataService
    {
        private readonly ILogger<PullDataService> _logger;
        private readonly IAppSettings _appSettings;
        private readonly IThirdPartyProductApi _thirdPartyProductApi;

        private const int MinRangeNumber = 1;

        private const string BetTransTicketToken = "$.BetTransTicket";
        private const string CountToken = "$.Count";

        public PullDataService(
            ILogger<PullDataService> logger,
            IAppSettings appSettings,
            IThirdPartyProductApi thirdPartyProductApi)
        {
            _logger = logger;
            _appSettings = appSettings;
            _thirdPartyProductApi = thirdPartyProductApi;
        }

        public async Task<IEnumerable<string>> GetRawDataListAsync(ThirdPartyApiParam param)
        {
            var rawDataList = new List<string>();
            var message = $"get raw data for: TpType={param.TpType}, SubItem={param.SubItem} and from {param.BeginDate.ToString(DateTimeFormats.Request)} to {param.EndDate.ToString(DateTimeFormats.Request)}";
            var pullRange = TimeSpan.FromMinutes(_appSettings.MaxTimeRange);
            var timeRanges = DateTimeHelper.GetProportionTimeRange(param.BeginDate, param.EndDate, pullRange);

            _logger.LogInformation($"Start {message}");

            foreach (var range in timeRanges)
            {
                param.BeginDate = range.Item1;
                param.EndDate = range.Item2;

                var rawData = await GetRawDataAsync(param);

                rawDataList.Add(rawData);
            }

            _logger.LogInformation($"Finished {message}");

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
                var message = $"get raw data for: TpType={param.TpType}, SubItem={param.SubItem} and from {param.BeginDate.ToString(DateTimeFormats.Request)} to {param.EndDate.ToString(DateTimeFormats.Request)}";
                var logBuilder = new StringBuilder();
                logBuilder.AppendLine($"Throw exception when {message}");
                logBuilder.AppendLine(LogHelper.BuildErrorMessage(ex));

                var logMessage = logBuilder.ToString();

                _logger.LogError(logMessage);

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

        public IEnumerable<JObject> GetUnescapedAndRemoveDataInfoDataList(string rawData)
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

        private IEnumerable<JObject> GetUnescapedDataByRange(JObject dataJObject, JArray betTransTicketList)
        {
            var maxItemsInRange = _appSettings.MaxItemsInRange;
            var rangeNumbers = betTransTicketList.Count / maxItemsInRange;
            var unescapedDataByRangeList = new List<JObject>();

            if (rangeNumbers < MinRangeNumber)
            {
                dataJObject.SelectToken(BetTransTicketToken).Replace(betTransTicketList);
                unescapedDataByRangeList.Add(dataJObject);
            }
            else
            {
                var betTransTicketChunks = betTransTicketList.Chunk(maxItemsInRange);
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

                var logMessage = LogHelper.BuildErrorMessage(ex);
                _logger.LogError(logMessage);

                throw;
            }
        }

        private IEnumerable<ThirdPartyProductInfo> GetConvertedDataByRange(IEnumerable<JObject> dataList)
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
