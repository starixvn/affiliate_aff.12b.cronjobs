
using System.Text.Json.Serialization;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.DTOs
{
    public class BaseThirdPartyProduct
    {
        [JsonPropertyName("Count")]
        public int Count { get; set; }

        [JsonPropertyName("VersionKey")]
        public string VersionKey { get; set; }

        [JsonPropertyName("SystemCode")]
        public string SystemCode { get; set; }

        [JsonPropertyName("SystemMessage")]
        public string SystemMessage { get; set; }
    }
}
