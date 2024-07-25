using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    //[Authorize]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    [Route("api/[controller]/[action]")]
    public class BtiController : ControllerBase
    {
        private readonly IBtiService _btiService;
        public BtiController(IBtiService btiService)
        {
            _btiService = btiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRawDataAsync([FromQuery] BaseApiParam param)
        {
            return Ok(await _btiService.GetRawDataListAsync(param));
        }

        [HttpGet]
        public async Task<IActionResult> GetConvertedDataAsync([FromQuery] BaseApiParam param)
        {
            return Ok(await _btiService.GetConvertedDataListAsync(param));
        }

        [HttpPost]
        public async Task<ActionResult> ImportDataToDbAsync([FromBody] BaseParam param)
        {
            if (ModelState.IsValid)
            {
                await _btiService.ImportDataToDbAsync(param);

                return new JsonResult("Import Data to Db successfully!");
            }

            return BadRequest(ModelState);
        }
    }
}
