using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Attributes;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Controller
{
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticateService _authService;

        public AuthController(IAuthenticateService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [Route("api/Auth")]
        [HttpPost]
        public ActionResult RequestToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }
            var result = _authService.IsAuthenticated(request);

            return result.Item1 ? Ok(result.Item2)
                : BadRequest("Wrong username or password");
        }
    }
}
