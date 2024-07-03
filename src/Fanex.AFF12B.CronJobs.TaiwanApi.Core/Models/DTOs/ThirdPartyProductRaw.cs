
using System.Text.Json.Serialization;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.DTOs
{
    public class ThirdPartyProductRaw : BaseThirdPartyProduct
    {
        [JsonPropertyName("BetTransTicket")]
        public string BetTransTicket { get; set; }
    }
}
