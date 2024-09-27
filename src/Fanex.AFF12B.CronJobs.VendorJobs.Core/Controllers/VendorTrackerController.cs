using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Fanex.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    [Route("api/vendor/[action]")]
    public class VendorTrackerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IVendorTrackerService _vendorTrackerService;

        public VendorTrackerController(
            ILogger logger,
            IVendorTrackerService vendorTrackerService)
        {
            _logger = logger;
            _vendorTrackerService = vendorTrackerService;
        }

        [HttpGet]
        public async Task<ActionResult> GetTimeTrackerAsync(string productId)
        {
            if (Product.ProductOnDb.ContainsKey(productId))
            {
                var timeTracker = await _vendorTrackerService.GetTrackerAsync(Convert.ToInt32(productId));

                return Ok($"{Product.GetProductNameById(productId)} Vendor - Time Tracker: {timeTracker}");
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTimeTrackerAsync(string productId, DateTime callTime)
        {
            if (Product.ProductOnDb.ContainsKey(productId))
            {
                await _vendorTrackerService.UpdateTrackerAsync(Convert.ToInt32(productId), callTime);

                var message = $"{Dns.GetHostName()} - Updated {Product.GetProductNameById(productId.ToString())}'s Vendor Tracker Time to {callTime.FormatDateTime()}";
                _logger.Info(message);

                return Ok($"{Product.GetProductNameById(productId)} Vendor - updated time tracker successfully!");
            }

            return BadRequest(ModelState);
        }
    }
}
