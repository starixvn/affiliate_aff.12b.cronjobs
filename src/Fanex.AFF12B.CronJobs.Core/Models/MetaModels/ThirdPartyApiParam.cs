using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class ThirdPartyApiParam : BaseParam
    {
        [Required]
        public DateTime BeginDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [JsonIgnore]
        [Range(0, 60)]
        public int MaxMinutesRange { get; set; }
    }
}
