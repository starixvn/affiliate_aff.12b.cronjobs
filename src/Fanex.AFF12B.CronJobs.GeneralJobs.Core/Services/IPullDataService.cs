using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.GeneralJobs.Core.Services
{
    public interface IPullDataService
    {
        Task<IEnumerable<ThirdPartyProductRaw>> GetRawDataListAsync(ThirdPartyApiParam param);

        Task<IEnumerable<ThirdPartyProductJson<T>>> GetJsonDataListAsync<T>(ThirdPartyApiParam param) where T : Ticket;
    }
}
