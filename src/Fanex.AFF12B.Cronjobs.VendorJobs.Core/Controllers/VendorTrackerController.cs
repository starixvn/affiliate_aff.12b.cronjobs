using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    public class VendorTrackerController : ControllerBase
    {
        private readonly IVendorTrackerService _vendorTrackerService;

        public VendorTrackerController(
            IVendorTrackerService vendorTrackerService)
        {
            _vendorTrackerService = vendorTrackerService;
        }

        [HttpGet("api/vendor/GetTimeTracker")]
        public async Task<ActionResult> GetTimeTrackerAsync(string productId)
        {
            if (Product.ProductOnDb.ContainsKey(productId))
            {
                var timeTracker = await _vendorTrackerService.GetTrackerAsync(Convert.ToInt32(productId));

                return Ok($"{Product.GetProductNameById(productId)} Vendor - Time Tracker: {timeTracker}");
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/vendor/UpdateTimeTracker")]
        public async Task<ActionResult> UpdateTimeTrackerAsync(string productId, DateTime callTime)
        {
            if (Product.ProductOnDb.ContainsKey(productId))
            {
                await _vendorTrackerService.UpdateTrackerAsync(Convert.ToInt32(productId), callTime);

                return Ok($"{Product.GetProductNameById(productId)} Vendor - updated time tracker successfully!");
            }

            return BadRequest(ModelState);
        }
    }
}
