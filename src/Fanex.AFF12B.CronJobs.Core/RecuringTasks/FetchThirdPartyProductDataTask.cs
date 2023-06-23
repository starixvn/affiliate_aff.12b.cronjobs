using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using ONEbook.UM.Services;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.RecuringTasks
{
    public class FetchThirdPartyProductDataTask : IFetchThirdPartyProductDataTask
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IImportDataService _importDataService;
        private readonly IMaintenanceService _maintenanceService;

        public FetchThirdPartyProductDataTask(
            ILogger logger,
            IConfiguration configuration,
            IImportDataService importDataService,
            IMaintenanceService maintenanceService)
        {
            _logger = logger;
            _configuration = configuration;
            _importDataService = importDataService;
            _maintenanceService = maintenanceService;
        }

        public async Task FetchDataAsync(string jobName, ThirdPartyCronJobSetting cronJobSetting)
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                _logger.Info("System is currently unavailable due to scheduled maintenance!");
                return;
            }

            var fromDate = await GetLastTimeFetchDateAsync(cronJobSetting);
            var toDate = GetAdjustmentApiServerDate(new TimeSpan(0, cronJobSetting.MinutesAdjustment, 0));

            var param = new ThirdPartyApiParam
            {
                BeginDate = fromDate,
                EndDate = toDate,
                TpType = cronJobSetting.TpType,
                SubItem = cronJobSetting.SubItem,
                MaxMinutesRange = cronJobSetting.MaxMinutesRange,
                MillisecondsDeplay = cronJobSetting.MillisecondsDeplay
            };

            await _importDataService.ImportDataToDbAsync(param, true);
        }

        private async Task<DateTime> GetLastTimeFetchDateAsync(ThirdPartyCronJobSetting cronJobSetting)
        {
            return await _importDataService.GetTimeTrackerAsync(cronJobSetting.TpType, cronJobSetting.SubItem);
        }

        private DateTime GetAdjustmentApiServerDate(TimeSpan ajustmentTime)
        {
            var date = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(_configuration.GetAppSettingsValue<int>("ApiServerTimeOffset")));
            var adjustDate = date.Add(ajustmentTime);

            return adjustDate.DateTime;
        }
    }
}
