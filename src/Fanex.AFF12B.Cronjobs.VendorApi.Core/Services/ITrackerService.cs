using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Services
{
    public interface ITrackerService
    {
        Task UpdateTrackerAsync(int productId, DateTime callTime);

        Task<DateTime> GetTrackerAsync(int productId);
    }
}
