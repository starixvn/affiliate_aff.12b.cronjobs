using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public interface IVendorService
    {
        Task<IEnumerable<string>> GetRawDataListAsync(BaseApiParam param);

        Task<IEnumerable<string>> GetXmlDataListAsync(BaseApiParam param);

        Task ImportDataToDbAsync(BaseApiParam param, bool isFromRunningJob = false);

        Task ImportDataToDbFromJobAsync(bool isFromRunningJob = false);
    }
}
