using Fanex.Athena.AFF12B.CronJobs.Core.Params;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Repositories
{
    public interface ITrackerRepository
    {
        Task AddTracker(TrackerParam param);

        Task<DateTime> GetTracker(int tpType, string? subItem = null);
    }
}
