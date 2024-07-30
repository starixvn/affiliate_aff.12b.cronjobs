using Newtonsoft.Json;
using System;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels
{
    [Serializable]
    public class BetsoftParam
    {
        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        [JsonProperty("end_date")]
        public string EndDate { get; set; }

        [JsonProperty("merchant_code")]
        public string MerchantCode { get; set; }
    }
}
