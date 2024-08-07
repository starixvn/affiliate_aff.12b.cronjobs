using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public class CrystalGpiService : VendorService, ICrystalGpiService
    {
        private readonly ICrystalGpiApi _crystalGpiApi;
        private readonly CrystalGpiSetting _crystalGpiSetting;
        private const string PRODUCT_NAME = "CrystalGpi";

        protected override string ErrorCodePropertyName => "errorCode";
        protected override string TicketsPropertyName => "bets";
        protected override string[] DataFieldNames => new string[] {
                "betId", "betNo", "memberId", "status", "actionTime",
                "timeBet", "currency", "betAmount", "winAmount",
            };

        public CrystalGpiService(
            ILogger logger,
            ICrystalGpiApi CrystalGpiApi,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<CrystalGpiSetting> CrystalGpiSetting) : base(logger, trackerService, importDataRepository, CrystalGpiSetting?.Value, PRODUCT_NAME)
        {
            _crystalGpiApi = CrystalGpiApi ?? throw new ArgumentNullException(nameof(CrystalGpiApi));
            _crystalGpiSetting = CrystalGpiSetting?.Value ?? throw new ArgumentNullException(nameof(CrystalGpiSetting));
        }

        protected override async Task<IEnumerable<string>> GetRawDataAsync(BaseParam param)
        {
            var rawDataList = new List<string>();
            var apiParam = BuildApiParam(param);
            var rawData = await _crystalGpiApi.GetRawDataAsync(apiParam);
            rawDataList.Add(ReFormatRawData(rawData));

            return rawDataList;
        }

        private CrystalGpiParam BuildApiParam(BaseParam param, int pageNumber = 1)
        {
            var startDate = param.From.ToString(_crystalGpiSetting.FormatDateTime);
            var endDate = param.To.ToString(_crystalGpiSetting.FormatDateTime);

            return new CrystalGpiParam
            {
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = pageNumber,
                VendorName = _crystalGpiSetting.VendorName,
                Password = _crystalGpiSetting.Password,
                Game = _crystalGpiSetting.Game,
                Status = _crystalGpiSetting.Status,
                PageSize = _crystalGpiSetting.PageSize,
            };
        }

        private string ReFormatRawData(string rawData)
        {
            var rawJsonArray = JArray.Parse(rawData);
            var isValidData = !rawJsonArray.HasValues
                || (rawJsonArray.HasValues && rawJsonArray.Any(j => !string.IsNullOrEmpty(j.SelectToken($"$.memberId").ToString())));

            if (isValidData)
            {
                var customData = $"{{\"{ErrorCodePropertyName}\":0, \"{TicketsPropertyName}\":{rawData}}}";
                return customData;
            }

            throw ErrorMessageHelper.FormatException($"{PRODUCT_NAME} Product Vendor - Data is invalid - {rawData}");
        }
    }
}
