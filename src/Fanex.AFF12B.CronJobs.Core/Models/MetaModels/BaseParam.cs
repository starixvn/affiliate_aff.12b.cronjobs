using Fanex.AFF12B.CronJobs.Core.Constant;
using System.ComponentModel.DataAnnotations;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class BaseParam
    {
        [Required]
        [Range(1, 100)]
        public byte TpType { get; set; }

        public string SubItem { get; set; } = DefaultValue.String;
    }
}
