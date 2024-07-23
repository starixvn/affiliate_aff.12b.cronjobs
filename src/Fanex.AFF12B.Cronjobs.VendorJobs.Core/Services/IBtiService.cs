using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public interface IBtiService
    {
        Task<IEnumerable<string>> GetRawDataListAsync(BaseApiParam param);

        Task ImportDataToDbAsync(bool isFromRunningJob = false);
    }
}
