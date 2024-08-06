using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Fanex.Logging;
using ONEbook.UM.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks
{
    public class FetchCrystalGpiProductDataTask : FetchVendorProductDataTask, IFetchVendorProductDataTask
    {
        public FetchCrystalGpiProductDataTask(
            ILogger logger,
            ICrystalGpiService crystalGpiService,
            IMaintenanceService maintenanceService) : base(logger, crystalGpiService, maintenanceService) { }
    }
}
