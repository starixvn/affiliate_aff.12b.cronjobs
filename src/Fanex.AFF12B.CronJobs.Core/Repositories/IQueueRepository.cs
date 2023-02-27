using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Repositories
{
    public interface IQueueRepository
    {
        Task AddQueue(QueueParam param);
    }
}
