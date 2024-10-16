using Fanex.AFF12B.CronJobs.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels
{
    public class BaseApiParam : BaseParam
    {
        private const int MaxMinutes = 60;
        private const int MaxMilliseconds = 120000;

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
