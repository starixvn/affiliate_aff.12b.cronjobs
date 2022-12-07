using Fanex.Athena.AFF12B.CronJobs.Jobs.Core.Configurations;
using Hangfire;

namespace Fanex.Athena.AFF12B.CronJobs.Jobs.Core.RecuringTasks
{
    public interface IFetchThirdPartyProductDataTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataAsync(ThirdPartyCronJobSetting cronJobSetting);
    }
}
