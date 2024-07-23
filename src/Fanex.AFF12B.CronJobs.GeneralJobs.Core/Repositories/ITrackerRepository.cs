using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.GeneralJobs.Core.Repositories
{
    public interface ITrackerRepository
    {
        Task AddTrackerAsync(TrackerParam param);

        Task<DateTime?> GetTrackerAsync(int tpType, string subItem = DefaultValue.String);
    }
}
