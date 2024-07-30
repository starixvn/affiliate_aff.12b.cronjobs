using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Fanex.Logging;
using ONEbook.UM.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks
{
    public class FetchBetsoftProductDataTask : FetchVendorProductDataTask
    {
        public FetchBetsoftProductDataTask(
            ILogger logger,
            IBetsoftService betsoftService,
            IMaintenanceService maintenanceService) : base(logger, betsoftService, maintenanceService) { }
    }
}
