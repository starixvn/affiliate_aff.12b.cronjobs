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
    public interface IDiamondEvoService : IVendorService {}

    public class DiamondEvoService : VendorService, IDiamondEvoService
    {
        private readonly IDiamondEvoApi _diamondEvoApi;
        private readonly DiamondEvoSetting _diamondEvoSetting;
        private const string PRODUCT_NAME = "DiamondEvo";

        protected override string ErrorCodePropertyName => "errorCode";
        protected override string TicketsPropertyName => "data";
        protected override string[] DataFieldNames => new string[] {
            "gamecode", "gamedate", "username", "currencycode", "bet","bonusbet", "winloss",
        };

        public DiamondEvoService(
            ILogger logger,
            IDiamondEvoApi diamondEvoApi,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<DiamondEvoSetting> diamondEvoSetting) : base(logger, trackerService, importDataRepository, diamondEvoSetting?.Value, PRODUCT_NAME)
        {
            _diamondEvoApi = diamondEvoApi ?? throw new ArgumentNullException(nameof(diamondEvoApi));
            _diamondEvoSetting = diamondEvoSetting?.Value ?? throw new ArgumentNullException(nameof(diamondEvoSetting));
        }

        protected override async Task<IEnumerable<string>> GetRawDataAsync(BaseParam param)
        {
            var rawDataList = new List<string>();
            var apiParam = BuildApiParam(param);
            var rawData = await _diamondEvoApi.GetRawDataAsync(apiParam);
            //rawDataList.Add(ReFormatRawData(rawData));
            rawDataList.Add(rawData);

            return rawDataList;
        }

        private DiamondEvoParam BuildApiParam(BaseParam param)
        {
            var startDate = param.From.ToString(_diamondEvoSetting.FormatDateTime);
            var endDate = param.To.ToString(_diamondEvoSetting.FormatDateTime);

            return new DiamondEvoParam
            {
                StartDate = startDate,
                EndDate = endDate
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