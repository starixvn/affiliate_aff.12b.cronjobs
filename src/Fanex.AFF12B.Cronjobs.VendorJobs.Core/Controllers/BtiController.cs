using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    public class BtiController : VendorController
    {
        public BtiController(IBtiService btiService) : base(btiService) { }
    }
}
