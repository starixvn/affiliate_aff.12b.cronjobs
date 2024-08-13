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
    public interface IPlaytechService : IVendorService {}

    public class PlaytechService : VendorService, IPlaytechService
    {
        private readonly IPlaytechApi _playtechApi;
        private readonly PlaytechSetting _playtechSetting;
        private const string PRODUCT_NAME = "Playtech";

        protected override string ErrorCodePropertyName => "errorCode";
        protected override string TicketsPropertyName => "data";
        protected override string[] DataFieldNames => new string[] {
            "gamecode", "gamedate", "username", "currencycode", "bet","bonusbet", "winloss",
        };

        public PlaytechService(
            ILogger logger,
            IPlaytechApi playtechApi,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<PlaytechSetting> playtechSetting) : base(logger, trackerService, importDataRepository, playtechSetting?.Value, PRODUCT_NAME)
        {
            _playtechApi = playtechApi ?? throw new ArgumentNullException(nameof(playtechApi));
            _playtechSetting = playtechSetting?.Value ?? throw new ArgumentNullException(nameof(playtechSetting));
        }

        protected override async Task<IEnumerable<string>> GetRawDataAsync(BaseParam param)
        {
            var rawDataList = new List<string>();
            var apiParam = BuildApiParam(param);
            var rawData = await _playtechApi.GetRawDataAsync(apiParam);
            rawDataList.Add(ReFormatRawData(rawData));

            return rawDataList;
        }

        private PlaytechParam BuildApiParam(BaseParam param)
        {
            var startDate = param.From.ToString(_playtechSetting.FormatDateTime);
            var endDate = param.To.ToString(_playtechSetting.FormatDateTime);

            return new PlaytechParam
            {
                StartDate = startDate,
                EndDate = endDate,
                FormatExtension = _playtechSetting.FormatExtension,
                ApiKey = _playtechSetting.ApiKey,
                Casino = _playtechSetting.Casino,
                SiAccounts = _playtechSetting.SiAccounts,
                Sdin = _playtechSetting.Sdin,
                ReportId = _playtechSetting.ReportId,
                ReportCode = _playtechSetting.ReportCode,
                Outputs = _playtechSetting.Outputs,
            };
        }

        private string ReFormatRawData(string rawData)
        {
            var rawJsonOject = JObject.Parse(rawData);
            var isValidData = rawJsonOject.HasValues && rawJsonOject.SelectToken($"$.{TicketsPropertyName}") != null;

            if (isValidData)
            {
                rawJsonOject.Remove("reportName");
                rawJsonOject.AddFirst(new JProperty(ErrorCodePropertyName, StatusCode.Success));
                return rawJsonOject.ToString();
            }

            throw ErrorMessageHelper.FormatException($"{PRODUCT_NAME} Product Vendor - Data is invalid - {rawData}");
        }
    }
}