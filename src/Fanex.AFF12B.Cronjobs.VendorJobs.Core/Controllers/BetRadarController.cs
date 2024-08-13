using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    public class BetRadarController : VendorController
    {
        public BetRadarController(IBetRadarService detRadarService) : base(detRadarService) { }
    }
}
