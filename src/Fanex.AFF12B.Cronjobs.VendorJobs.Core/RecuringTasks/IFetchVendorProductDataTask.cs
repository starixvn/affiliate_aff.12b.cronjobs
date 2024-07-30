using Hangfire;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks
{
    public interface IFetchVendorProductDataTask
    {
        [DisableConcurrentExecution(30)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataAsync();
    }
}
