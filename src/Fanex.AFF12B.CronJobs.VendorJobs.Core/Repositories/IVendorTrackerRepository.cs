using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.Commands;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories
{
    public interface IVendorTrackerRepository
    {
        Task UpdateTrackerAsync(UpTrackerCommand command);

        Task<DateTime?> GetTrackerAsync(TrackerCriteria criteria);
    }
}
