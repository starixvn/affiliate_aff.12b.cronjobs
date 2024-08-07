using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public abstract class VendorService
    {
        private readonly ILogger _logger;
        private readonly IVendorTrackerService _trackerService;
        private readonly IImportVendorDataRepository _importDataRepository;
        private readonly VendorSetting _vendorSetting;
        private readonly string _productName;

        protected virtual string ErrorCodePropertyName => string.Empty;
        protected virtual string TicketsPropertyName => string.Empty;
        protected virtual string[] DataFieldNames => new string[] { };

        public VendorService(
            ILogger logger,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            VendorSetting vendorSetting,
            string productName)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _trackerService = trackerService ?? throw new ArgumentNullException(nameof(trackerService));
            _importDataRepository = importDataRepository ?? throw new ArgumentNullException(nameof(importDataRepository));
            _vendorSetting = vendorSetting ?? throw new ArgumentNullException(nameof(vendorSetting));
            _productName = productName;
        }

        public async Task<IEnumerable<string>> GetXmlDataListAsync(BaseApiParam param)
        {
            return await GetDataListAsync(param, HandleGetXmlDataAsync);
        }

        public async Task<IEnumerable<string>> GetRawDataListAsync(BaseApiParam param)
        {
            return await GetDataListAsync(param, HandleGetRawDataAsync);
        }

        private async Task<IEnumerable<string>> GetDataListAsync(BaseApiParam param, Func<BaseApiParam, Task<IEnumerable<string>>> getDataAsync)
        {
            var dataList = new List<string>();
            var timeRanges = GetTimeRanges(param);

            foreach (var range in timeRanges)
            {
                param.From = range.Item1;
                param.To = range.Item2;

                var dataRange = await getDataAsync(param);

                dataList.AddRange(dataRange);
            }

            return dataList;
        }

        public async Task ImportDataToDbFromJobAsync(bool isFromRunningJob = false)
        {
            var fromDate = await _trackerService.GetTrackerAsync(_vendorSetting.ProductId, _vendorSetting.StartTime);
            var toDate = DateTimeHelper.GetAdjustmentApiServerDate(new TimeSpan(0, _vendorSetting.MinutesAdjustment, 0));
            var param = new BaseApiParam
            {
                From = fromDate,
                To = toDate,
            };

            await ImportDataToDbAsync(param, isFromRunningJob);
        }

        public async Task ImportDataToDbAsync(BaseApiParam param, bool isFromRunningJob = false)
        {
            var logBuilder = new StringBuilder();
            var message = $"import {Product.GetProductNameById(_vendorSetting.ProductId.ToString())} - from {param.From.FormatDateTime()} to {param.To.FormatDateTime()}";

            try
            {
                _logger.Info($"{ServerHelper.ServerName} - Starting {message}");

                var timeRanges = GetTimeRanges(param);

                foreach (var range in timeRanges)
                {
                    await ImportDataByTimeRangeToDbAsync(range.Item1, range.Item2, isFromRunningJob).ConfigureAwait(false);
                }

                _logger.Info($"{ServerHelper.ServerName} - Finished {message}");
            }
            catch (Exception ex)
            {
                logBuilder.AppendLine($"{ServerHelper.ServerName} - {ErrorMessageHelper.GenerateMessage(ex, null, $"Range {message}")}");

                var logMessage = logBuilder.ToString();

                await _logger.ErrorAsync(logMessage);
            }
        }

        private async Task ImportDataByTimeRangeToDbAsync(DateTime from, DateTime to, bool isFromRunningJob)
        {
            var xmlDatas = await HandleGetXmlDataAsync(new BaseApiParam { From = from, To = to });

            foreach (var xmlData in xmlDatas)
            {
                await _importDataRepository.ImportDataAsync(_vendorSetting.ProductId, xmlData.ToString());
            }

            if (isFromRunningJob)
            {
                await _trackerService.UpdateTrackerAsync(_vendorSetting.ProductId, to);
            }
            else
            {
                var productName = Product.GetProductNameById(_vendorSetting.ProductId.ToString());
                _logger.Info($"{ServerHelper.ServerName} - Finished manually import {productName} Product Vendor - from {from.FormatDateTime()} to {to.FormatDateTime()}");
            }
        }

        private async Task<IEnumerable<string>> HandleGetXmlDataAsync(BaseApiParam param)
        {
            var dataList = new List<string>();
            var rawDatas = await HandleGetRawDataAsync(param);

            foreach (var data in rawDatas)
            {
                var xmlData = ConvertRawDataToXml(data);
                var root = xmlData.Root;
                var shouldImportToDb = root.Elements(TicketsPropertyName).Count() != 0;

                if (shouldImportToDb)
                {
                    dataList.Add(xmlData.ToString());
                }
            }

            return dataList;
        }

        private IEnumerable<Tuple<DateTime, DateTime>> GetTimeRanges(BaseApiParam param)
        {
            var pullRange = TimeSpan.FromMinutes(param.MaxMinutesRange == 0 ? _vendorSetting.MaxMinutesRange : param.MaxMinutesRange);

            return DateTimeHelper.GetProportionTimeRange(param.From, param.To, pullRange);
        }

        private async Task<IEnumerable<string>> HandleGetRawDataAsync(BaseApiParam param)
        {
            await Task.Delay(param.MillisecondsDeplay == 0 ? _vendorSetting.MillisecondsDeplay : param.MillisecondsDeplay, CancellationToken.None);

            return await GetRawDataAsync(param);
        }

        protected abstract Task<IEnumerable<string>> GetRawDataAsync(BaseParam param);

        private XDocument ConvertRawDataToXml(string raw)
        {
            var formatedJson = ReFormatJson(raw);
            var xml = XmlHelper.ParseToXDocumentFromJson(formatedJson);

            return xml is null
                ? throw ErrorMessageHelper.FormatException($"{_productName} Product Vendor - Can't convert json response to xml - {raw}")
                : CheckXml(xml);
        }

        private string ReFormatJson(string raw)
        {
            var rawJsonOject = JObject.Parse(raw);
            var code = rawJsonOject.SelectToken($"$.{ErrorCodePropertyName}");

            if ((string)code == StatusCode.Success)
            {
                var dataList = rawJsonOject.SelectToken($"$.{TicketsPropertyName}");
                var items = ReArrangeDataToListItems(dataList);
                dataList.Replace(items);
            }

            return rawJsonOject.ToString();
        }

        private JArray ReArrangeDataToListItems(JToken dataList)
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

        private void FillDataToItem(JObject item, JToken betItem)
        {
            foreach (var pro in DataFieldNames)
            {
                var data = betItem.SelectToken($"$.{pro}");

                item.Add(pro, data);
            }
        }

        private XDocument CheckXml(XDocument xml)
        {
            var root = xml.Root;
            var code = root.Element(ErrorCodePropertyName) ?? throw ErrorMessageHelper.FormatException($"{_productName} Product Vendor - Xml response is not correct format {xml}");

            if (code.Value != StatusCode.Success)
            {
                throw ErrorMessageHelper.FormatException($"{_productName} Product Vendor - Xml response code indicate request not success {xml}");
            }

            return xml;
        }
    }
}
