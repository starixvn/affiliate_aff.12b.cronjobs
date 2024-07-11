using Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.Commands;
using Fanex.AFF12B.Cronjobs.VendorApi.Core.Repositories;
using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels;
using Fanex.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Services
{
    public class TrackerService : ITrackerService
    {
        private readonly ILogger _logger;
        private readonly ITrackerRepository _trackerRepository;
        public TrackerService(
            ILogger logger,
            ITrackerRepository trackerRepository)
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
