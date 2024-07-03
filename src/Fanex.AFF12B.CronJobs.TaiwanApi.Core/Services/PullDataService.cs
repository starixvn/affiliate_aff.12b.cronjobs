using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Apis;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Services
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

        public async Task<IEnumerable<ThirdPartyProductJson<T>>> GetJsonDataListAsync<T>(ThirdPartyApiParam param) where T : Ticket
        {
            var rawDataList = await GetRawDataListAsync(param);
            var convertedDataList = new List<ThirdPartyProductJson<T>>();

            foreach (var rawData in rawDataList)
            {
                convertedDataList.AddRange(GetJsonDataRemovedDataInfoList<T>(rawData, param));
            }

            return convertedDataList;
        }

        private async Task<ThirdPartyProductRaw> GetRawDataAsync(ThirdPartyApiParam param)
        {
            return await _thirdPartyProductApi.GetTPGBetTrans(param);
        }

        private static List<ThirdPartyProductJson<T>> GetJsonDataRemovedDataInfoList<T>(ThirdPartyProductRaw rawData, ThirdPartyApiParam param) where T : Ticket
        {
            var productName = Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem);

            if (rawData is null)
            {
                throw ErrorMessageHelper.FormatException($"{productName} - Nothing respond from Api", param);
            }

            var systemCodeToken = rawData.SystemCode;
            var isCodeSuccess = systemCodeToken.Equals(CODE_SUCCESS) || systemCodeToken.Equals(CODE_SUCCESS_NO_TICKET) || systemCodeToken.Equals(CODE_SUCCESS_NO_VALID_TICKET);
            if (string.IsNullOrWhiteSpace(systemCodeToken) || !isCodeSuccess)
            {
                var message = $"{productName} - The 3rd api return ErrorCode: {systemCodeToken}, ErrorMessage: {rawData.SystemMessage}";

                throw ErrorMessageHelper.FormatException(message, param);
            }

            var betTransTicketJToken = rawData.BetTransTicket;
            if (string.IsNullOrWhiteSpace(betTransTicketJToken))
            {
                return new List<ThirdPartyProductJson<T>> { new ThirdPartyProductJson<T> {
                    BetTransTicket = new List<T>(),
                    Count = rawData.Count,
                    VersionKey = rawData.VersionKey,
                    SystemCode = rawData.SystemCode,
                    SystemMessage = rawData.SystemMessage
                }};
            }

            var betTransTicketList = JArray.Parse(betTransTicketJToken);
            List<T> newBetTransTicketList = new List<T>();

            foreach (var item in betTransTicketList)
            {
                var ticket = JObject.Parse(item.ToString());

                ticket.Remove("datainfo");

                newBetTransTicketList.Add(JsonConvert.DeserializeObject<T>(ticket.ToString()));
            }
            var jsonData = GetThirdPartyProductJsonObject<T>(rawData);

            return GetJsonDataRemovedDataInfoByTicketRange(jsonData, newBetTransTicketList);
        }

        private static List<ThirdPartyProductJson<T>> GetJsonDataRemovedDataInfoByTicketRange<T>(ThirdPartyProductJson<T> jsonData, List<T> betTransTicketList) where T : Ticket
        {
            var totalTickets = betTransTicketList.Count;
            var rangeNumbers = (totalTickets + MAX_TICKETS_IN_RANGE) / MAX_TICKETS_IN_RANGE;
            var unescapedDataByRangeList = new List<ThirdPartyProductJson<T>>();

            if (rangeNumbers == MIN_RANGE_NUMBER)
            {
                jsonData.BetTransTicket = betTransTicketList;
                unescapedDataByRangeList.Add(jsonData);
            }
            else
            {
                for (int i = 0; i < rangeNumbers; i++)
                {
                    var newObjectData = GetThirdPartyProductJsonObject<T>(jsonData);
                    var ticketNumber = i == (rangeNumbers - 1) ? (totalTickets - (i * MAX_TICKETS_IN_RANGE)) : MAX_TICKETS_IN_RANGE;
                    var ticketList = betTransTicketList.GetRange(i * MAX_TICKETS_IN_RANGE, ticketNumber);

                    newObjectData.BetTransTicket = ticketList;
                    newObjectData.Count = ticketNumber;

                    unescapedDataByRangeList.Add(newObjectData);
                }
            }

            return unescapedDataByRangeList;
        }

        private static ThirdPartyProductJson<T> GetThirdPartyProductJsonObject<T>(BaseThirdPartyProduct rawData) where T : Ticket
        {
            return new ThirdPartyProductJson<T>
            {
                Count = rawData.Count,
                VersionKey = rawData.VersionKey,
                SystemCode = rawData.SystemCode,
                SystemMessage = rawData.SystemMessage
            };
        }
    }
}
