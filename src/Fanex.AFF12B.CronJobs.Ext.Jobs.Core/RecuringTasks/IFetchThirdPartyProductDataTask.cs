using Fanex.AFF12B.CronJobs.Ext.Jobs.Core.Configurations;
using Hangfire;

namespace Fanex.AFF12B.CronJobs.Ext.Jobs.Core.RecuringTasks
{
    public interface IFetchThirdPartyProductDataTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataAsync(ThirdPartyCronJobSetting cronJobSetting);
    }
}
