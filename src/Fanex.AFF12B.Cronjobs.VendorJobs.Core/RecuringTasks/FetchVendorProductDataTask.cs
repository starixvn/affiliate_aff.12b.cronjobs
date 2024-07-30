using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Fanex.Logging;
using ONEbook.UM.Services;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks
{
    public abstract class FetchVendorProductDataTask : IFetchVendorProductDataTask
    {
        private readonly ILogger _logger;
        private readonly IVendorService _vendorService;
        private readonly IMaintenanceService _maintenanceService;
        public FetchVendorProductDataTask(
            ILogger logger,
            IVendorService vendorService,
            IMaintenanceService maintenanceService)
        {
            _logger = logger;
            _vendorService = vendorService;
            _maintenanceService = maintenanceService;
        }

        public async Task FetchDataAsync()
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                _logger.Info("System is currently unavailable due to scheduled maintenance!");
                return;
            }

            await _vendorService.ImportDataToDbFromJobAsync(true);
        }
    }
}
