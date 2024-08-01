using Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public class PlaytechService : VendorService, IPlaytechService
    {
        private readonly IPlaytechApi _playtechApi;
        private readonly PlaytechSetting _playtechSetting;
        private const string PRODUCT_NAME = "Playtech";

        protected override string ErrorCodePropertyName => "status";
        protected override string BetsPropertyName => "details";
        protected override string[] DataFieldNames => new string[] {
                "gameid", "gamecode", "gamedate", "username",
                "bonusbet", "winloss", "currencycode", "progressiveshare", "totalwin", "jackpotwin",
            };

        public PlaytechService(
            ILogger logger,
            IPlaytechApi PlaytechApi,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<PlaytechSetting> PlaytechSetting) : base(logger, trackerService, importDataRepository, PlaytechSetting?.Value, PRODUCT_NAME)
        {
            _playtechApi = PlaytechApi ?? throw new ArgumentNullException(nameof(PlaytechApi));
            _playtechSetting = PlaytechSetting?.Value ?? throw new ArgumentNullException(nameof(PlaytechSetting));
        }

        protected override async Task<IEnumerable<string>> GetRawDataAsync(BaseParam param)
        {
            var rawDataList = new List<string>();
            var apiParam = BuildApiParam(param);
            var rawData = await _playtechApi.GetRawDataAsync(apiParam);
            rawDataList.Add(rawData.ToString());

            return rawDataList;
        }

        private PlaytechParam BuildApiParam(BaseParam param)
        {
            var startDate = param.From.ToString(_playtechSetting.FormatDateTime);
            var endDate = param.To.ToString(_playtechSetting.FormatDateTime);

            return new PlaytechParam { 
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
    }
}