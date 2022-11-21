using Fanex.Athena.AFF12B.CronJobs.Core.Params;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Repositories
{
    internal interface IQueueRepository
    {
        Task AddQueue(QueueParam param);
    }
}
