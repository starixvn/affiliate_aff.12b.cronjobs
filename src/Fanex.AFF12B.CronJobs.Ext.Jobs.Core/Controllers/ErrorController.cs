using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Ext.Jobs.Core.Controllers
{
    public class ErrorController : ControllerBase
    {
        public ActionResult Index()
        {
            return NotFound("Page not found");
        }
    }
}
