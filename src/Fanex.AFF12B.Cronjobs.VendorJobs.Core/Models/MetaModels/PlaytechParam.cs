using Newtonsoft.Json;
using System;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels
{
    [Serializable]
    public class PlaytechParam
    {
        [JsonProperty("startdate")]
        public string StartDate { get; set; }
        [JsonProperty("enddate")]
        public string EndDate { get; set; }
        [JsonProperty("format")]
        public string FormatExtension { get; set; }
        [JsonProperty("apikey")]
        public string ApiKey { get; set; }
        [JsonProperty("casino")]
        public string Casino { get; set; }
        [JsonProperty("siaccounts")]
        public string SiAccounts { get; set; }
        [JsonProperty("sdin")]
        public string Sdin { get; set; }
        [JsonProperty("reportid")]
        public string ReportId { get; set; }
        [JsonProperty("reportcode")]
        public string ReportCode { get; set; }
        [JsonProperty("outputs")]
        public string Outputs { get; set; }
    }
}
