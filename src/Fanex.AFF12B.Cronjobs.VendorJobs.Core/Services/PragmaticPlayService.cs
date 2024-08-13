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
    public interface IPragmaticPlayService : IVendorService {}

    public class PragmaticPlayService : VendorService, IPragmaticPlayService
    {
        private readonly IPragmaticPlayApi _pragmaticPlayApi;
        private readonly PragmaticPlaySetting _pragmaticPlaySetting;
        private const string PRODUCT_NAME = "PragmaticPlay";

        protected override string ErrorCodePropertyName => "errorCode";
        protected override string TicketsPropertyName => "data";
        protected override string[] DataFieldNames => new string[] {
            "gamecode", "gamedate", "username", "currencycode", "bet","bonusbet", "winloss",
        };

        public PragmaticPlayService(
            ILogger logger,
            IPragmaticPlayApi pragmaticPlayApi,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<PragmaticPlaySetting> pragmaticPlaySetting) : base(logger, trackerService, importDataRepository, pragmaticPlaySetting?.Value, PRODUCT_NAME)
        {
            _pragmaticPlayApi = pragmaticPlayApi ?? throw new ArgumentNullException(nameof(pragmaticPlayApi));
            _pragmaticPlaySetting = pragmaticPlaySetting?.Value ?? throw new ArgumentNullException(nameof(pragmaticPlaySetting));
        }

        protected override async Task<IEnumerable<string>> GetRawDataAsync(BaseParam param)
        {
            var rawDataList = new List<string>();
            var apiParam = BuildApiParam(param);
            var rawData = await _pragmaticPlayApi.GetRawDataAsync(apiParam);
            //rawDataList.Add(ReFormatRawData(rawData));
            rawDataList.Add(rawData);

            return rawDataList;
        }

        private PragmaticPlayParam BuildApiParam(BaseParam param)
        {
            var startDate = param.From.ToString(_pragmaticPlaySetting.FormatDateTime);
            var endDate = param.To.ToString(_pragmaticPlaySetting.FormatDateTime);
            var timePoint = new DateTimeOffset(param.From).ToUnixTimeSeconds();

            return new PragmaticPlayParam
            {
                Login = _pragmaticPlaySetting.Username,
                Password = _pragmaticPlaySetting.Password,
                TimePoint = timePoint,
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