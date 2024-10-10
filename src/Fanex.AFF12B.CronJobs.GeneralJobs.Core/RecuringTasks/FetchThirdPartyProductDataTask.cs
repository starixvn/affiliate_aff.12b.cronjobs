using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Services;
using Fanex.Logging;
using ONEbook.UM.Services;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.GeneralJobs.Core.RecuringTasks
{
    public class FetchThirdPartyProductDataTask : IFetchThirdPartyProductDataTask
    {
        private readonly ILogger _logger;
        private readonly IImportDataService _importDataService;
        private readonly IMaintenanceService _maintenanceService;

        public FetchThirdPartyProductDataTask(
            ILogger logger,
            IImportDataService importDataService,
            IMaintenanceService maintenanceService)
        {
            _logger = logger;
            _importDataService = importDataService;
            _maintenanceService = maintenanceService;
        }

        public async Task FetchDataAsync(string jobName, ThirdPartyProductSetting cronJobSetting)
        {
            await FetchDataAsync<Ticket>(cronJobSetting);
        }

        public async Task FetchConvertedTransactionIdDataAsync(string jobName, ThirdPartyProductSetting cronJobSetting)
        {
            await FetchDataAsync<ConvertedTransactionIdTicket>(cronJobSetting);
        }

        public async Task FetchAdjustedDateTimeDataAsync(string jobName, AdjustedDateTimeProductSetting cronJobSetting)
        {
            await FetchDataAsync<AdjustedDateTimeTicket>(cronJobSetting);
        }

        public async Task FetchDataAsync<T>(ThirdPartyProductSetting cronJobSetting) where T : Ticket
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                _logger.Info("System is currently unavailable due to scheduled maintenance!");
                return;
            }

            var fromDate = await GetLastTimeFetchDateAsync(cronJobSetting);
            var toDate = DateTimeHelper.GetAdjustmentApiServerDate(new TimeSpan(0, cronJobSetting.MinutesAdjustment, 0));

            var param = new ThirdPartyApiParam
            {
                BeginDate = fromDate,
                EndDate = toDate,
                TpType = cronJobSetting.TpType,
                SubItem = cronJobSetting.SubItem,
                MaxMinutesRange = cronJobSetting.MaxMinutesRange,
                MillisecondsDeplay = cronJobSetting.MillisecondsDeplay
            };

            if (cronJobSetting.GetType().Name == typeof(AdjustedDateTimeProductSetting).Name)
            {
                var setting = cronJobSetting as AdjustedDateTimeProductSetting;
                param.HoursAdjustment = setting.HoursAdjustment;
                param.FormatDateTime = setting.FormatDateTime;
            }

            await _importDataService.ImportDataToDbAsync<T>(param, true);
        }

        private async Task<DateTime> GetLastTimeFetchDateAsync(ThirdPartyProductSetting cronJobSetting)
        {
            return await _importDataService.GetTimeTrackerAsync(cronJobSetting.TpType, cronJobSetting.SubItem);
        }
    }
}
