using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class ThirdPartyApiParam : BaseParam
    {
        private const int MinHoursAdjustment = -12;
        private const int MaxHoursAdjustment = 12;
        private const int MaxMinutes = 60;
        private const int MaxMilliseconds = 120000;

        [Required]
        public DateTime BeginDate { get; set; }

        [Required]
        [JsonIgnore]
        public DateTime EndDate { get; set; }

        [JsonIgnore]
        [Range(0, MaxMinutes)]
        public int MaxMinutesRange { get; set; }

        [JsonIgnore]
        [Range(0, MaxMilliseconds)]
        public int MillisecondsDeplay { get; set; }

        [JsonIgnore]
        [Range(MinHoursAdjustment, MaxHoursAdjustment)]
        public int HoursAdjustment { get; set; }

        public string FormatDateTime { get; set; }
    }
}
