using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.Athena.AFF12B.CronJobs.Core.Services;
using Fanex.Athena.AFF12B.CronJobs.Jobs.Core.Configurations;
using Fanex.Logging;
using ONEbook.UM.Services;

namespace Fanex.Athena.AFF12B.CronJobs.Jobs.Core.RecuringTasks
{
    public class FetchThirdPartyProductDataTask : IFetchThirdPartyProductDataTask
    {
        private const int API_SERVER_TIME_OFFSET = 0;
        private readonly IImportDataService _importDataService;
        private readonly IMaintenanceService _maintenanceService;

        public FetchThirdPartyProductDataTask(
            IImportDataService importDataService,
            IMaintenanceService maintenanceService)
        {
            _importDataService = importDataService;
            _maintenanceService = maintenanceService;
        }

        public async Task FetchDataAsync(ThirdPartyCronJobSetting cronJobSetting)
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

            Logger.Log.Info($"{cronJobSetting.CronJobName} is started to import data from {fromDate} to {toDate}");
            await _importDataService.ImportDataAndTrackTimeToDbAsync(param);
        }

        private async Task<DateTime> GetLastTimeFetchDateAsync(ThirdPartyCronJobSetting cronJobSetting)
        {
            var trackingDate = await _importDataService.GetTimeTrackerAsync(cronJobSetting.TpType, cronJobSetting.SubItem);
            var lastFetchDate = trackingDate.AddMinutes(cronJobSetting.MinutesAdjustment);

            return lastFetchDate;
        }

        private static DateTime GetAdjustmentApiServerDate()
        {
            var date = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(API_SERVER_TIME_OFFSET));
            return date.DateTime;
        }
    }
}
