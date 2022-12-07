using Fanex.Athena.AFF12B.CronJobs.Core.Constant;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class QueueParam : BaseParam
    {
        public byte JobType { get; set; }

        public string JsonData { get; set; } = DefaultValue.String;
    }
}
