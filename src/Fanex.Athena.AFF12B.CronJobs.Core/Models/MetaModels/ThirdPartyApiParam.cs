using Fanex.Athena.AFF12B.CronJobs.Core.Constant;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        public string SubItem { get; set; } = DefaultValue.String;

        [JsonIgnore]
        [Range(0, 60)]
        public int MaxMinutesRange { get; set; }
    }
}
