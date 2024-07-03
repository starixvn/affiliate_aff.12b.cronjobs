using Newtonsoft.Json;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.DTOs
{
    public class Ticket
    {
        [JsonProperty("bet")]
        public string Bet { get; set; }

        [JsonProperty("betstatus")]
        public string BetStatus { get; set; }

        [JsonProperty("gameplatform")]
        public string GamePlatform { get; set; }

        [JsonProperty("ishedgebet")]
        public string IsHedgeBet { get; set; }

        [JsonProperty("thirdpartyid")]
        public string ThirdPartyId { get; set; }

        [JsonProperty("transactiondatetime")]
        public string TransactiondateTime { get; set; }

        [JsonProperty("transactionid")]
        public string TransactionId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("validbet")]
        public string ValidBet { get; set; }

        [JsonProperty("win")]
        public string Win { get; set; }
    }
}
