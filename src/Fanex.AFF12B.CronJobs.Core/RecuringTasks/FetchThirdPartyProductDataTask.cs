using Fanex.AFF12B.CronJobs.Core.Contants;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using ONEbook.UM.Services;
using System.Threading.Tasks;
using System;
using System.Net;

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
                MaxMinutesRange = cronJobSetting.MaxMinutesRange,
                MillisecondsDeplay = cronJobSetting.MillisecondsDeplay
            };

            var message = $"import { Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} from { param.BeginDate.FormatDateTime()} to { param.EndDate.FormatDateTime()}";
            Logger.Log.Info($"{Dns.GetHostName()} - Starting {message}");

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
