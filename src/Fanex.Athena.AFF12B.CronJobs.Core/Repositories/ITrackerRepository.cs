using Fanex.Athena.AFF12B.CronJobs.Core.Constant;
using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Repositories
{
    public interface ITrackerRepository
    {
        Task AddTracker(TrackerParam param);

        Task<DateTime> GetTracker(int tpType, string subItem = DefaultValue.String);
    }
}
