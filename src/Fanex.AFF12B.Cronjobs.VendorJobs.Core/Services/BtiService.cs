using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public class BtiService : IBtiService
    {
        private const string BTI_TOKEN_MEMORY_CACHE = "BTI_TOKEN_MEMORY_CACHE";
        private const int BTI_CACHE_TIME_OUT = 20;

        private readonly ILogger _logger;
        private readonly IBtiApi _btiApi;
        private readonly IMemoryCache _memoryCache;
        private readonly IVendorTrackerService _trackerService;
        private readonly IImportVendorDataRepository _importDataRepository;
        private readonly BtiSetting _btiSetting;
        private static readonly string SERVER_NAME = Dns.GetHostName();

        public BtiService(
            ILogger logger,
            IBtiApi btiApi,
            IMemoryCache memoryCache,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<BtiSetting> btiSetting)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _btiApi = btiApi ?? throw new ArgumentNullException(nameof(btiApi));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _trackerService = trackerService ?? throw new ArgumentNullException(nameof(trackerService));
            _importDataRepository = importDataRepository ?? throw new ArgumentNullException(nameof(importDataRepository));
            _btiSetting = btiSetting?.Value ?? throw new ArgumentNullException(nameof(btiSetting));
        }

        public async Task<IEnumerable<string>> GetConvertedDataListAsync(BaseApiParam param)
        {
            var rawDataList = new List<string>();
            var pullRange = GetPullRange(param.MaxMinutesRange);
            var timeRanges = DateTimeHelper.GetProportionTimeRange(param.From, param.To, pullRange);

            foreach (var range in timeRanges)
            {
                param.From = range.Item1;
                param.To = range.Item2;

                await Task.Delay(_btiSetting.MillisecondsDeplay, CancellationToken.None);
                var xmlData = await GetXmlDataAsync(param);

                rawDataList.Add(xmlData.ToString());
            }

            return rawDataList;
        }

        public async Task ImportDataToDbAsync(BaseParam param, bool isFromRunningJob = false)
        {
            var logBuilder = new StringBuilder();
            var message = $"import {Product.GetProductNameById(_btiSetting.ProductId.ToString())} from {param.From.FormatDateTime()} to {param.To.FormatDateTime()}";

            try
            {
                _logger.Info($"{SERVER_NAME} - Starting {message}");

                var pullRange = TimeSpan.FromMinutes(_btiSetting.MaxMinutesRange);
                var timeRanges = DateTimeHelper.GetProportionTimeRange(param.From, param.To, pullRange);

                foreach (var range in timeRanges)
                {
                    await Task.Delay(_btiSetting.MillisecondsDeplay, CancellationToken.None);
                    await ImportDataByTimeRangeToDbAsync(range.Item1, range.Item2, isFromRunningJob).ConfigureAwait(false);
                }

                _logger.Info($"{SERVER_NAME} - Finished {message}");
            }
            catch (Exception ex)
            {
                logBuilder.AppendLine($"{SERVER_NAME} - {ErrorMessageHelper.GenerateMessage(ex, null, $"Range {message}")}");

                var logMessage = logBuilder.ToString();

                await _logger.ErrorAsync(logMessage);
            }
        }

        public async Task ImportDataToDbFromJobAsync(bool isFromRunningJob = false)
        {
            var fromDate = await _trackerService.GetTrackerAsync(_btiSetting.ProductId);
            var toDate = DateTimeHelper.GetAdjustmentApiServerDate(new TimeSpan(0, _btiSetting.MinutesAdjustment, 0));
            var param = new BaseParam
            {
                From = fromDate,
                To = toDate,
            };

            await ImportDataToDbAsync(param, isFromRunningJob);
        }

        private async Task ImportDataByTimeRangeToDbAsync(DateTime from, DateTime to, bool isFromRunningJob)
        {
            var xmlData = await GetXmlDataAsync(new BaseApiParam { From = from, To = to });

            await _importDataRepository.ImportDataAsync(_btiSetting.ProductId, xmlData.ToString());

            if (isFromRunningJob)
            {
                await _trackerService.UpdateTrackerAsync(_btiSetting.ProductId, to);
            }
            else
            {
                var productName = Product.GetProductNameById(_btiSetting.ProductId.ToString());
                _logger.Info($"{SERVER_NAME} - Finished manually import {productName} from {from.FormatDateTime()} to {to.FormatDateTime()}");
            }
        }

        public async Task<IEnumerable<string>> GetRawDataListAsync(BaseApiParam param)
        {
            var rawDataList = new List<string>();
            var pullRange = GetPullRange(param.MaxMinutesRange);
            var timeRanges = DateTimeHelper.GetProportionTimeRange(param.From, param.To, pullRange);

            foreach (var range in timeRanges)
            {
                param.From = range.Item1;
                param.To = range.Item2;

                await Task.Delay(_btiSetting.MillisecondsDeplay, CancellationToken.None);
                var rawData = await GetRawDataAsync(param);

                rawDataList.Add(rawData);
            }

            return rawDataList;
        }

        private TimeSpan GetPullRange(int maxMinutesRange)
        {
            return TimeSpan.FromMinutes(maxMinutesRange == 0 ? _btiSetting.MaxMinutesRange : maxMinutesRange);
        }

        private async Task<XDocument> GetXmlDataAsync(BaseParam param)
        {
            var rawData = await GetRawDataAsync(param);

            return ConvertRawDataToXml(rawData);
        }

        private async Task<string> GetRawDataAsync(BaseParam param)
        {
            var tokenInfo = await HandleGetTokenAsync();

            if (tokenInfo is null || !tokenInfo.IsValidToken)
            {
                throw ErrorMessageHelper.FormatException("BTi Product - cannot get a token!", param);
            }

            return await _btiApi.GetRawDataAsync(tokenInfo.Token, param);
        }

        private async Task<BtiToken> HandleGetTokenAsync()
        {
            var token = await _memoryCache.GetOrCreateAsync(BTI_TOKEN_MEMORY_CACHE, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(BTI_CACHE_TIME_OUT);
                var tokenParam = new BtiTokenParam
                {
                    AgentUsername = _btiSetting.AgentUsername,
                    AgentPassword = _btiSetting.AgentPassword,
                };

                return await _btiApi.GetTokenAsync(tokenParam);
            });

            return token;
        }

        private static XDocument ConvertRawDataToXml(string raw)
        {
            var formatedJson = ReFormatJson(raw);
            var xml = XmlHelper.ParseToXDocumentFromJson(formatedJson);

            return xml is null
                ? throw ErrorMessageHelper.FormatException($"BTi Product - Can't convert json response to xml - {raw}")
                : CheckXml(xml);
        }

        private static string ReFormatJson(string raw)
        {
            const string successCode = "0";
            var rawJsonOject = JObject.Parse(raw);
            var code = rawJsonOject.SelectToken("$.errorCode");

            if ((string)code == successCode)
            {
                var dataList = rawJsonOject.SelectToken("$.Bets");
                var items = ReArrangeDataToListItems(dataList);
                dataList.Replace(items);
            }

            return rawJsonOject.ToString();
        }

        private static JArray ReArrangeDataToListItems(JToken dataList)
        {
            var items = new JArray();
            for (int i = 0; i < dataList.Count(); i++)
            {
                var item = new JObject();
                FillDataToItem(item, dataList[i]);
                items.Add(item);
            }
            return items;
        }

        private static void FillDataToItem(JObject item, JToken betItem)
        {
            foreach (var pro in GetDataFieldNames())
            {
                var data = betItem.SelectToken($"$.{pro}");

                item.Add(pro, data);
            }
        }
        private static string[] GetDataFieldNames()
            =>
            new string[] {
                "PurchaseID", "UserName", "TotalStake", "ValidStake",
                "PL", "BetStatus", "BetSettledDate",
            };

        public static XDocument CheckXml(XDocument xml)
        {
            var root = xml.Root;
            var code = root.Element("errorCode") ?? throw ErrorMessageHelper.FormatException($"BTi Product - Xml response is not correct format {xml}");

            const string successCode = "0";
            var value = code.Value;

            if (value != successCode)
            {
                throw ErrorMessageHelper.FormatException($"BTi Product - Xml response code indicate request not success {xml}");
            }

            return xml;
        }
    }
}
