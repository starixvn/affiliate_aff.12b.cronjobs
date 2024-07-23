using Hangfire;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks
{
    public interface IFetchVendorProductDataTask
    {
        [DisplayName("{0}")]
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataAsync();
    }
}
