using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using Fanex.Data;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.GeneralJobs.Core.Repositories
{
    public class TrackerRepository : ITrackerRepository
    {
        public async Task AddTrackerAsync(TrackerParam param)
        {
            const string SpName = "Task_APIProduct_CallTimeIns";

            using var db = ObjectDbFactory.CreateInstance(SpName);

            await db.ExecuteNonQueryAsync(param);
        }

        public async Task<DateTime?> GetTrackerAsync(int tpType, string subItem = DefaultValue.String)
        {
            const string SpName = "Task_APIProduct_CallTimeByProductSel";

            using var db = ObjectDbFactory.CreateInstance(SpName);
            db.SetParameterValue(nameof(tpType), tpType);
            db.SetParameterValue(nameof(subItem), subItem);

            return await db.QueryEntityAsync<DateTime?>();
        }
    }
}
