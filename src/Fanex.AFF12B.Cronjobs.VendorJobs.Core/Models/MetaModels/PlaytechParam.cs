using Newtonsoft.Json;
using System;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels
{
    [Serializable]
    public class PlaytechParam
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        [JsonProperty("format")]
        public string FormatExtension { get; set; }
        public string ApiKey { get; set; }
        public string Casino { get; set; }
        public string SiAccounts { get; set; }
        public string Sdin { get; set; }
        public string ReportId { get; set; }
        public string ReportCode { get; set; }
        public string Outputs { get; set; }
    }
}
