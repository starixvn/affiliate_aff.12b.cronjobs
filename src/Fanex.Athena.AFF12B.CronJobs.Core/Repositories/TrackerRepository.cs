using Fanex.Athena.AFF12B.CronJobs.Core.Constant;
using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.Data;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Repositories
{
    public class TrackerRepository : ITrackerRepository
    {
        public Task AddTracker(TrackerParam param)
        {
            const string SpName = "Task_APIProduct_CallTimeIns";

            using var db = ObjectDbFactory.CreateInstance(SpName);

            return db.ExecuteNonQueryAsync(param);
        }

        public Task<DateTime> GetTracker(int tpType, string subItem = DefaultValues.String)
        {
            const string SpName = "Task_APIProduct_CallTimeByProductSel";

            using var db = ObjectDbFactory.CreateInstance(SpName);

            return db.QueryEntityAsync<DateTime>(new { tpType, subItem });
        }
    }
}
