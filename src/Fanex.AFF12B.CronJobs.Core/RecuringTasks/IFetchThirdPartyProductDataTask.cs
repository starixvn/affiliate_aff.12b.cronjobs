using Fanex.AFF12B.CronJobs.Core.Configurations;
using Hangfire;
using System.ComponentModel;

namespace Fanex.AFF12B.CronJobs.Core.RecuringTasks
{
    public interface IFetchThirdPartyProductDataTask
    {
        [DisplayName("{0}")]
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataAsync(string jobName, ThirdPartyProductSetting cronJobSetting);

        [DisplayName("{0}")]
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchConvertedTransactionIdDataAsync(string jobName, ThirdPartyProductSetting cronJobSetting);

        [DisplayName("{0}")]
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchAdjustedDateTimeDataAsync(string jobName, AdjustedDateTimeProductSetting cronJobSetting);
    }
}
