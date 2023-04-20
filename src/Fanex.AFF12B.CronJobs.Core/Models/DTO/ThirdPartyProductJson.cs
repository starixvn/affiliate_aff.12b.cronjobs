using System.Collections.Generic;

namespace Fanex.AFF12B.CronJobs.Core.Models.DTO
{
    public class ThirdPartyProductJson : BaseThirdPartyProduct
    {
        public IEnumerable<Ticket> BetTransTicket { get; set; }
    }
}
