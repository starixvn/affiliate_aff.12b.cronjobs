namespace Fanex.AFF12B.CronJobs.Core.Services
{
    using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
    using Models.MetaModels;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPullDataService
    {
        Task<IEnumerable<ThirdPartyProductRaw>> GetRawDataListAsync(ThirdPartyApiParam param);

        Task<IEnumerable<ThirdPartyProductJson<T>>> GetJsonDataListAsync<T>(ThirdPartyApiParam param);
    }
}
