using Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.MetaModels;
using Fanex.AFF12B.Cronjobs.VendorApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BtiController : ControllerBase
    {
        private readonly IBtiService _btiService;
        public BtiController(IBtiService btiService)
        {
            _btiService = btiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRawDataAsync([FromQuery]BaseApiParam param)
        {
            return Ok(await _btiService.GetRawDataListAsync(param));
        }
    }
}
