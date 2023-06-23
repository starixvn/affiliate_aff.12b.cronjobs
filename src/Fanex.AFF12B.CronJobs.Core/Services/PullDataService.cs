using Fanex.AFF12B.CronJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.Core.Contants;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public class PullDataService : IPullDataService
    {
        private readonly IThirdPartyProductApi _thirdPartyProductApi;

        private const int MIN_RANGE_NUMBER = 1;
        private const int MAX_TICKETS_IN_RANGE = 2000;

        private const string CODE_SUCCESS = "GT_S_000";
        private const string CODE_SUCCESS_NO_TICKET = "GT_S_001";
        private const string CODE_SUCCESS_NO_VALID_TICKET = "GT_S_003";

        public PullDataService(
            IThirdPartyProductApi thirdPartyProductApi)
        {
            _thirdPartyProductApi = thirdPartyProductApi;
        }

        public async Task<IEnumerable<ThirdPartyProductRaw>> GetRawDataListAsync(ThirdPartyApiParam param)
        {
            var rawDataList = new List<ThirdPartyProductRaw>();
            var pullRange = TimeSpan.FromMinutes(param.MaxMinutesRange);
            var timeRanges = DateTimeHelper.GetProportionTimeRange(param.BeginDate, param.EndDate, pullRange);

            foreach (var range in timeRanges)
            {
                param.BeginDate = range.Item1;
                param.EndDate = range.Item2;

                var rawData = await GetRawDataAsync(param);

                rawDataList.Add(rawData);
            }

            return rawDataList;
        }

        public async Task<IEnumerable<ThirdPartyProductJson>> GetJsonDataListAsync(ThirdPartyApiParam param)
        {
            var rawDataList = await GetRawDataListAsync(param);
            var convertedDataList = new List<ThirdPartyProductJson>();

            foreach (var rawData in rawDataList)
            {
                convertedDataList.AddRange(GetJsonDataRemovedDataInfoList(rawData, param));
            }

            return convertedDataList;
        }

        private async Task<ThirdPartyProductRaw> GetRawDataAsync(ThirdPartyApiParam param)
        {
            return await _thirdPartyProductApi.GetTPGBetTrans(param);
        }

        private IEnumerable<ThirdPartyProductJson> GetJsonDataRemovedDataInfoList(ThirdPartyProductRaw rawData, ThirdPartyApiParam param)
        {
            var productName = Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem);

            if (rawData is null)
            {
                throw CustomException($"{productName} - Nothing respond from Api", param);
            }

            var systemCodeToken = rawData.SystemCode;
            var isCodeSuccess = systemCodeToken.Equals(CODE_SUCCESS) || systemCodeToken.Equals(CODE_SUCCESS_NO_TICKET) || systemCodeToken.Equals(CODE_SUCCESS_NO_VALID_TICKET);
            if (string.IsNullOrWhiteSpace(systemCodeToken) || !isCodeSuccess)
            {
                var message = $"{productName} - The 3rd api return ErrorCode: {systemCodeToken}, ErrorMessage: {rawData.SystemMessage}";

                throw CustomException(message, param);
            }

            var betTransTicketJToken = rawData.BetTransTicket;
            if (string.IsNullOrWhiteSpace(betTransTicketJToken))
            {
                return new List<ThirdPartyProductJson> { new ThirdPartyProductJson {
                    BetTransTicket = new List<Ticket>(),
                    Count = rawData.Count,
                    VersionKey = rawData.VersionKey,
                    SystemCode = rawData.SystemCode,
                    SystemMessage = rawData.SystemMessage
                }};
            }

            var betTransTicketList = JArray.Parse(betTransTicketJToken);
            List<Ticket> newBetTransTicketList = new List<Ticket>();

            foreach (var item in betTransTicketList)
            {
                var ticket = JObject.Parse(item.ToString());

                ticket.Remove("datainfo");

                newBetTransTicketList.Add(JsonConvert.DeserializeObject<Ticket>(ticket.ToString()));
            }
            var jsonData = GetThirdPartyProductJsonObject(rawData);

            return GetJsonDataRemovedDataInfoByTicketRange(jsonData, newBetTransTicketList);
        }

        private static IEnumerable<ThirdPartyProductJson> GetJsonDataRemovedDataInfoByTicketRange(ThirdPartyProductJson jsonData, List<Ticket> betTransTicketList)
        {
            var totalTickets = betTransTicketList.Count;
            var rangeNumbers = (totalTickets + MAX_TICKETS_IN_RANGE) / MAX_TICKETS_IN_RANGE;
            var unescapedDataByRangeList = new List<ThirdPartyProductJson>();

            if (rangeNumbers == MIN_RANGE_NUMBER)
            {
                jsonData.BetTransTicket = betTransTicketList;
                unescapedDataByRangeList.Add(jsonData);
            }
            else
            {
                for (int i = 0; i < rangeNumbers; i++)
                {
                    var newObjectData = GetThirdPartyProductJsonObject(jsonData);
                    var ticketNumber = i == (rangeNumbers - 1) ? (totalTickets - (i * MAX_TICKETS_IN_RANGE)) : MAX_TICKETS_IN_RANGE;
                    var ticketList = betTransTicketList.GetRange(i * MAX_TICKETS_IN_RANGE, ticketNumber);

                    newObjectData.BetTransTicket = ticketList;
                    newObjectData.Count = ticketNumber;

                    unescapedDataByRangeList.Add(newObjectData);
                }
            }

            return unescapedDataByRangeList;
        }

        private static ThirdPartyProductJson GetThirdPartyProductJsonObject(BaseThirdPartyProduct rawData)
        {
            return new ThirdPartyProductJson
            {
                Count = rawData.Count,
                VersionKey = rawData.VersionKey,
                SystemCode = rawData.SystemCode,
                SystemMessage = rawData.SystemMessage
            };
        }

        private static Exception CustomException(string initialMessage, ThirdPartyApiParam param)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(initialMessage);
            messageBuilder.Append($"Parameters: {JsonConvert.SerializeObject(param)}");

            return new FormatException(messageBuilder.ToString());
        }
    }
}
