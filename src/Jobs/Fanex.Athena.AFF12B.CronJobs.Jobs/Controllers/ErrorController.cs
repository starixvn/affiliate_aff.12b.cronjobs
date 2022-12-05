using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.AFF12B.CronJobs.Jobs.Core.Controllers
{
    public class ErrorController : ControllerBase
    {
        public ActionResult Index()
        {
            return NotFound("Page not found");
        }
    }
}
