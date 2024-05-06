using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class AuthController(IAuthenticateService authService) : ControllerBase
    {
        [AllowAnonymous]
        [Route("api/Auth")]
        [HttpPost]
        public ActionResult RequestToken([FromBody] TokenRequestParam request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }
            var result = authService.IsAuthenticated(request);

            if (result.Item1)
            {
                return Ok(result.Item2);
            }

            return BadRequest("Wrong username or password");
        }
    }
}
