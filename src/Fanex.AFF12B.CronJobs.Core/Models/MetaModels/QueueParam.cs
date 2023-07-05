using Fanex.AFF12B.CronJobs.Core.Constants;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class QueueParam : BaseParam
    {
        public string JsonData { get; set; } = DefaultValue.String;
    }
}
