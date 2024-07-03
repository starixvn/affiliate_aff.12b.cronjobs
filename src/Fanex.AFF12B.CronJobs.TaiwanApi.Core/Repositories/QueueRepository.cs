using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels;
using Fanex.Data;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        public async Task AddQueueAsync(QueueParam param)
        {
            const string SpName = "Task_NewAPIProduct_ImportQueue";

            using var db = ObjectDbFactory.CreateInstance(SpName);

            await db.ExecuteNonQueryAsync(param);
        }
    }
}
