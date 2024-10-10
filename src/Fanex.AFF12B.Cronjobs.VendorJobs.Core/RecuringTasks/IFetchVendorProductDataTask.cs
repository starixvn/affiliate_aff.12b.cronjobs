using Hangfire;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks
{
    [DisableConcurrentExecution(60)]
    public interface IFetchVendorProductDataTask
    {
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataAsync();
    }
}
