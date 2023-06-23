using Fanex.AFF12B.CronJobs.Core.Constant;
using System.ComponentModel.DataAnnotations;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class BaseParam
    {
        [Required]
        [Range(1, 100, ErrorMessage = "TpType is invalid!")]
        public byte TpType { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SubItem { get; set; } = DefaultValue.String;
    }
}
