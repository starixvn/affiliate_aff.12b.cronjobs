using Fanex.AFF12B.CronJobs.Core.Contants;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using ONEbook.UM.Services;

namespace Fanex.AFF12B.CronJobs.Core.RecuringTasks
{
    public class FetchThirdPartyProductDataTask : IFetchThirdPartyProductDataTask
    {
        private readonly IConfiguration _configuration;
        private readonly IImportDataService _importDataService;
        private readonly IMaintenanceService _maintenanceService;

        public FetchThirdPartyProductDataTask(
            IConfiguration configuration,
            IImportDataService importDataService,
            IMaintenanceService maintenanceService)
        {
            _configuration = configuration;
            _importDataService = importDataService;
            _maintenanceService = maintenanceService;
        }

        public async Task FetchDataAsync(string jobName, ThirdPartyCronJobSetting cronJobSetting)
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                Logger.Log.Info("System is currently unavailable due to scheduled maintenance!");
                return;
            }

            var fromDate = await GetLastTimeFetchDateAsync(cronJobSetting);
            var toDate = GetAdjustmentApiServerDate();

            var param = new ThirdPartyApiParam
            {
                BeginDate = fromDate,
                EndDate = toDate,
                TpType = cronJobSetting.TpType,
                SubItem = cronJobSetting.SubItem,
                MaxMinutesRange = cronJobSetting.MaxMinutesRange
            };

            Logger.Log.Info($"{Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} is starting to import data from {fromDate} to {toDate}");
            await _importDataService.ImportDataToDbAsync(param, true);
        }

        private async Task<DateTime> GetLastTimeFetchDateAsync(ThirdPartyCronJobSetting cronJobSetting)
        {
            var trackingDate = await _importDataService.GetTimeTrackerAsync(cronJobSetting.TpType, cronJobSetting.SubItem);
            var lastFetchDate = trackingDate.AddMinutes(cronJobSetting.MinutesAdjustment);

            return lastFetchDate;
        }

        private DateTime GetAdjustmentApiServerDate()
        {
            var date = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(_configuration.GetAppSettingsValue<int>("ApiServerTimeOffset")));
            return date.DateTime;
        }
    }
}
