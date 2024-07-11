using Fanex.AFF12B.Cronjobs.VendorApi.Core.Services;
using Fanex.Logging;
using ONEbook.UM.Services;
using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.RecuringTasks
{
    public class FetchBtiProductDataTask : IFetchVendorProductDataTask
    {
        private readonly ILogger _logger;
        private readonly IBtiService _btiService;
        private readonly IMaintenanceService _maintenanceService;
        public FetchBtiProductDataTask(
            ILogger logger,
            IBtiService btiService,
            IMaintenanceService maintenanceService)
        {
            _logger = logger;
            _btiService = btiService;
            _maintenanceService = maintenanceService;
        }

        public async Task FetchDataAsync()
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                _logger.Info("System is currently unavailable due to scheduled maintenance!");
                return;
            }

            await _btiService.ImportDataToDbAsync(true);
        }
    }
}
