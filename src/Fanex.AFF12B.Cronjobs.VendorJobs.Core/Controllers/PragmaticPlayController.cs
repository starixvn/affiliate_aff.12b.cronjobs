using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    public class PragmaticPlayController : VendorController
    {
        public PragmaticPlayController(IPragmaticPlayService pragmaticPlayService) : base(pragmaticPlayService) { }
    }
}
