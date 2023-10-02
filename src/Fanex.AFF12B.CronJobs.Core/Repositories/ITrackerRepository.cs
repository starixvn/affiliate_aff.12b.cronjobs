using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Repositories
{
    public interface ITrackerRepository
    {
        Task AddTracker(TrackerParam param);

        Task<DateTime?> GetTracker(int tpType, string subItem = DefaultValue.String);
    }
}
