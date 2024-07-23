using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.Commands;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public class VendorTrackerService : IVendorTrackerService
    {
        private readonly ILogger _logger;
        private readonly IVendorTrackerRepository _trackerRepository;
        public VendorTrackerService(
            ILogger logger,
            IVendorTrackerRepository trackerRepository)
        {
            _logger = logger;
            _trackerRepository = trackerRepository;
        }

        public async Task<DateTime> GetTrackerAsync(int productId)
        {
            var timeTracker = await _trackerRepository.GetTrackerAsync(new TrackerCriteria { ProductId = productId });

            if (timeTracker is null)
            {
                var now = DateTimeOffset.Now;
                return new DateTimeOffset(now.Year, now.Month, 01, 00, 00, 00, TimeSpan.Zero).DateTime;
            }

            return (DateTime)timeTracker;
        }

        public async Task UpdateTrackerAsync(int productId, DateTime callTime)
        {
            await _trackerRepository.UpdateTrackerAsync(
                new UpTrackerCommand
                {
                    ProductId = productId,
                    CallTime = callTime
                });

            var message = $"{Dns.GetHostName()} - Updated {Product.GetProductNameById(productId.ToString())}'s Tracker Time to {callTime.FormatDateTime()}";
            _logger.Info(message);
        }
    }
}
