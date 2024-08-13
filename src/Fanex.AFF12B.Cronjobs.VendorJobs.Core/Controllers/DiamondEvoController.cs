using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    public class DiamondEvoController : VendorController
    {
        public DiamondEvoController(IDiamondEvoService diamondEvoService) : base(diamondEvoService) { }
    }
}
