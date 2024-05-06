using System.Text.Json.Serialization;

namespace Fanex.AFF12B.CronJobs.Core.Models.DTOs
{
    public class ThirdPartyProductJson<T> : BaseThirdPartyProduct where T : Ticket
    {
        [JsonPropertyName("BetTransTicket")]
        public IEnumerable<T> BetTransTicket { get; set; }
    }
}
