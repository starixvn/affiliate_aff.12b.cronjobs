using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public interface IVendorTrackerService
    {
        Task UpdateTrackerAsync(int productId, DateTime callTime);

        Task<DateTime> GetTrackerAsync(int productId);
    }
}
