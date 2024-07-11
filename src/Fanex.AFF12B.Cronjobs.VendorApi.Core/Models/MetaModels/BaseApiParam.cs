using Fanex.AFF12B.CronJobs.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.MetaModels
{
    public class BaseApiParam
    {
        private const int MaxMinutes = 60;
        private const int MaxMilliseconds = 120000;

        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }

        [JsonIgnore]
        [Range(0, MaxMinutes)]
        public int MaxMinutesRange { get; set; }

        [JsonIgnore]
        [Range(0, MaxMilliseconds)]
        public int MillisecondsDeplay { get; set; }

        [JsonIgnore]
        public string FormatDateTime { get; set; } = Format.DefaultFomartDateTime;
    }
}
