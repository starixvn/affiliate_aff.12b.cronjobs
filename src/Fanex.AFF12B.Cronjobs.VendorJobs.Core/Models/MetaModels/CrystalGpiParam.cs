using Newtonsoft.Json;
using System;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels
{
    [Serializable]
    public class CrystalGpiParam
    {
        [JsonProperty("sDate")]
        public string StartDate { get; set; }
        [JsonProperty("eDate")]
        public string EndDate { get; set; }
        public string Game { get; set; }
        public string Status { get; set; }
        public string VendorName { get; set; }
        public string Password { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
