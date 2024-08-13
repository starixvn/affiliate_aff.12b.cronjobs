using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public interface IBetRadarService : IVendorService {}

    public class BetRadarService : VendorService, IBetRadarService
    {
        private readonly IBetRadarApi _betRadarApi;
        private readonly BetRadarSetting _betRadarSetting;
        private const string PRODUCT_NAME = "BetRadar";

        protected override string ErrorCodePropertyName => "errorCode";
        protected override string TicketsPropertyName => "data";
        protected override string[] DataFieldNames => new string[] {
            "gamecode", "gamedate", "username", "currencycode", "bet","bonusbet", "winloss",
        };

        public BetRadarService(
            ILogger logger,
            IBetRadarApi betRadarApi,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<BetRadarSetting> betRadarSetting) : base(logger, trackerService, importDataRepository, betRadarSetting?.Value, PRODUCT_NAME)
        {
            _betRadarApi = betRadarApi ?? throw new ArgumentNullException(nameof(betRadarApi));
            _betRadarSetting = betRadarSetting?.Value ?? throw new ArgumentNullException(nameof(betRadarSetting));
        }

        protected override async Task<IEnumerable<string>> GetRawDataAsync(BaseParam param)
        {
            var rawDataList = new List<string>();
            var apiParam = BuildApiParam(param);
            var rawData = await _betRadarApi.GetRawDataAsync(apiParam);
            //rawDataList.Add(ReFormatRawData(rawData));
            rawDataList.Add(rawData);

            return rawDataList;
        }

        private BetRadarParam BuildApiParam(BaseParam param)
        {
            var startDate = param.From.ToString(_betRadarSetting.FormatDateTime);
            var endDate = param.To.ToString(_betRadarSetting.FormatDateTime);

            return new BetRadarParam
            {
                DateFrom = startDate,
                DateTo = endDate,
                OriginId = _betRadarSetting.OriginId,
                LineId = _betRadarSetting.LineId,
            };
        }

        private string ReFormatRawData(string rawData)
        {
            var rawJsonOject = JObject.Parse(rawData);
            var isValidData = rawJsonOject.HasValues && rawJsonOject.SelectToken($"$.{TicketsPropertyName}") != null;

            if (isValidData)
            {
                rawJsonOject.AddFirst(new JProperty(ErrorCodePropertyName, StatusCode.Success));
                return rawJsonOject.ToString();
            }

            throw ErrorMessageHelper.FormatException($"{PRODUCT_NAME} Product Vendor - Data is invalid - {rawData}");
        }
    }
}