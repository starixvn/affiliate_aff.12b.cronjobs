using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.Data;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Repositories
{
    public class TrackerRepository : ITrackerRepository
    {
        public Task AddTracker(TrackerParam param)
        {
            const string SpName = "Task_APIProduct_CallTimeIns";

            using var db = ObjectDbFactory.CreateInstance(SpName);

            return db.ExecuteNonQueryAsync(param);
        }

        public Task<DateTime?> GetTracker(int tpType, string subItem = DefaultValue.String)
        {
            const string SpName = "Task_APIProduct_CallTimeByProductSel";

            using var db = ObjectDbFactory.CreateInstance(SpName);
            db.SetParameterValue(nameof(tpType), tpType);
            db.SetParameterValue(nameof(subItem), subItem);

            return db.QueryEntityAsync<DateTime?>();
        }
    }
}
