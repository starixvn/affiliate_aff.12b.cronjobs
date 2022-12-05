using Fanex.Athena.AFF12B.CronJobs.Core.Constant;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class BaseParam
    {
        public int TpType { get; set; }

        public string SubItem { get; set; } = DefaultValue.String;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; } = DefaultValue.String;
    }
}
