using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fanex.AFF12B.CronJobs.Core.Models.DTOs
{
    public class ThirdPartyProductJson : BaseThirdPartyProduct
    {
        [JsonPropertyName("BetTransTicket")]
        public IEnumerable<Ticket> BetTransTicket { get; set; }
    }
}
