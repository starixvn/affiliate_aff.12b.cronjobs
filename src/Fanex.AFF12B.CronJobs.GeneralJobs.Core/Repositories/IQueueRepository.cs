using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.GeneralJobs.Core.Repositories
{
    public interface IQueueRepository
    {
        Task AddQueueAsync(QueueParam param);
    }
}
