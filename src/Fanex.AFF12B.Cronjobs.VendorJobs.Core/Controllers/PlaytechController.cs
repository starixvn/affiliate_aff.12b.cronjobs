using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    public class PlaytechController : VendorController
    {
        public PlaytechController(IPlaytechService playtechService) : base(playtechService) { }
    }
}
