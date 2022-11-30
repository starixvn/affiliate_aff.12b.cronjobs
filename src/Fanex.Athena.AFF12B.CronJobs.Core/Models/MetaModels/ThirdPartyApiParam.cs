using Fanex.Athena.AFF12B.CronJobs.Core.Constant;
using System.ComponentModel.DataAnnotations;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class ThirdPartyApiParam
    {
        [Required]
        public DateTime BeginDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, 100)]
        public byte TpType { get; set; }

        public string SubItem { get; set; } = DefaultValues.String;
    }
}
