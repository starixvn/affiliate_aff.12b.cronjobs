using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Controllers
{
    public class ErrorController : ControllerBase
    {
        public ActionResult Index(int code)
        {
            return code == (int)HttpStatusCode.Unauthorized ? Unauthorized("You don't have permission to access") : NotFound("Page not found");
        }
    }
}
