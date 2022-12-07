using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.Data;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        public Task AddQueue(QueueParam param)
        {
            const string SpName = "Task_NewAPIProduct_ImportQueue";

            using var db = ObjectDbFactory.CreateInstance(SpName);

            return db.ExecuteNonQueryAsync(param);
        }
    }
}
