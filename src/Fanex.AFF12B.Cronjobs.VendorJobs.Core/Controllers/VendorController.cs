using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    [Route("api/[controller]/[action]")]
    public abstract class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService ?? throw new ArgumentNullException(nameof(vendorService));
        }

        [HttpGet]
        public async Task<IActionResult> GetRawDataAsync([FromQuery] BaseApiParam param)
        {
            return Ok(await _vendorService.GetRawDataListAsync(param));
        }

        [HttpGet]
        public async Task<IActionResult> GetConvertedDataAsync([FromQuery] BaseApiParam param)
        {
            return Ok(await _vendorService.GetConvertedDataListAsync(param));
        }

        [HttpPost]
        public async Task<ActionResult> ImportDataToDbAsync([FromBody] BaseApiParam param)
        {
            if (ModelState.IsValid)
            {
                await _vendorService.ImportDataToDbAsync(param);

                return new JsonResult("Import Data to Db successfully!");
            }

            return BadRequest(ModelState);
        }
    }
}
