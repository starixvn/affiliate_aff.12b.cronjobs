using Fanex.Athena.AFF12B.CronJobs.Core.Constant;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class TrackerParam : BaseParam
    {
        public int Currency { get; set; }

        public DateTime CallTime { get; set; }
    }
}
