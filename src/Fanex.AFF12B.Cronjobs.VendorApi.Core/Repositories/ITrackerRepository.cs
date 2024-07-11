using Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.Commands;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Repositories
{
    public interface ITrackerRepository
    {
        Task UpdateTrackerAsync(UpTrackerCommand command);

        Task<DateTime?> GetTrackerAsync(TrackerCriteria criteria);
    }
}
