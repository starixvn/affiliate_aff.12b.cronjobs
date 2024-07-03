using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Repositories
{
    public interface IQueueRepository
    {
        Task AddQueueAsync(QueueParam param);
    }
}
