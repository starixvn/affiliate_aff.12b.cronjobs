using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    public class ErrorController : ControllerBase
    {
        public ActionResult Index(int code)
        {
            if (code == (int)HttpStatusCode.Unauthorized)
            {
                return Unauthorized("You don't have permission to access");
            }
            return NotFound("Page not found");
        }
    }
}
