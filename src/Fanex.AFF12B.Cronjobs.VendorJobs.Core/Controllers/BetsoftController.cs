using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    public class BetsoftController : VendorController
    {
        public BetsoftController(IBetsoftService betsoftService) : base(betsoftService) { }
    }
}
