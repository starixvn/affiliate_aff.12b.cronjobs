using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using ONEbook.UM.Services;

namespace Fanex.AFF12B.CronJobs.Core.RecuringTasks
{
    public class FetchThirdPartyProductDataTask(
        ILogger logger,
        IConfiguration configuration,
        IImportDataService importDataService,
        IMaintenanceService maintenanceService) : IFetchThirdPartyProductDataTask
    {
        public async Task FetchDataAsync(string jobName, ThirdPartyProductSetting cronJobSetting)
        {
            await HandleFetchDataAsync<Ticket>(cronJobSetting);
        }

        public async Task FetchConvertedTransactionIdDataAsync(string jobName, ThirdPartyProductSetting cronJobSetting)
        {
            await HandleFetchDataAsync<ConvertedTransactionIdTicket>(cronJobSetting);
        }

        public async Task FetchAdjustedDateTimeDataAsync(string jobName, AdjustedDateTimeProductSetting cronJobSetting)
        {
            await HandleFetchDataAsync<AdjustedDateTimeTicket>(cronJobSetting);
        }

        private async Task HandleFetchDataAsync<T>(ThirdPartyProductSetting cronJobSetting) where T : Ticket
        {
            if (maintenanceService.IsUnderMaintenance())
            {
                logger.Info("System is currently unavailable due to scheduled maintenance!");
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

            if (cronJobSetting.GetType().Name == typeof(AdjustedDateTimeProductSetting).Name)
            {
                var setting = cronJobSetting as AdjustedDateTimeProductSetting;
                param.HoursAdjustment = setting.HoursAdjustment;
                param.FormatDateTime = setting.FormatDateTime;
            }

            await importDataService.ImportDataToDbAsync<T>(param, true);
        }

        private async Task<DateTime> GetLastTimeFetchDateAsync(ThirdPartyProductSetting cronJobSetting)
        {
            return await importDataService.GetTimeTrackerAsync(cronJobSetting.TpType, cronJobSetting.SubItem);
        }

        private DateTime GetAdjustmentApiServerDate(TimeSpan ajustmentTime)
        {
            var date = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(configuration.GetAppSettingsValue<int>("ApiServerTimeOffset")));
            var adjustDate = date.Add(ajustmentTime);

            return adjustDate.DateTime;
        }
    }
}
