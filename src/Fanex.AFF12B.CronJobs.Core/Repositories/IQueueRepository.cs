using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;

namespace Fanex.AFF12B.CronJobs.Core.Repositories
{
    public interface IQueueRepository
    {
        Task AddQueue(QueueParam param);
    }
}
