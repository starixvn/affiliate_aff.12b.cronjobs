using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Fanex.Logging;
using ONEbook.UM.Services;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks
{
    public class FetchBtiProductDataTask : FetchVendorProductDataTask
    {
        public FetchBtiProductDataTask(
            ILogger logger,
            IBtiService btiService,
            IMaintenanceService maintenanceService) : base(logger, btiService, maintenanceService) { }
    }
}
