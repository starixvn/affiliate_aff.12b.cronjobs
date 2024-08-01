using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    public class CrystalGpiController : VendorController
    {
        public CrystalGpiController(ICrystalGpiService crystalGpiService) : base(crystalGpiService) { }
    }
}
