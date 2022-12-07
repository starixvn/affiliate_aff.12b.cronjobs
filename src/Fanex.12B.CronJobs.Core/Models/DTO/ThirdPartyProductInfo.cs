
namespace Fanex.Athena.AFF12B.CronJobs.Core.Models.DTO
{
    public class ThirdPartyProductInfo
    {
        public IEnumerable<Ticket> BetTransTicket { get; set; }

        public int Count { get; set; }

        public string Tickets { get; set; }

        public string VersionKey { get; set; }

        public string SystemCode { get; set; }

        public string SystemMessage { get; set; }
    }
}
