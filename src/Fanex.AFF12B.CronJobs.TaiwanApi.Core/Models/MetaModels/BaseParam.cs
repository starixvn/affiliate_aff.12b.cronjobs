using Fanex.AFF12B.CronJobs.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels
{
    public class BaseParam
    {
        private const int MaxTpType = 100;

        [Required]
        [Range(1, MaxTpType, ErrorMessage = "TpType is invalid!")]
        public byte TpType { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SubItem { get; set; } = DefaultValue.String;
    }
}
