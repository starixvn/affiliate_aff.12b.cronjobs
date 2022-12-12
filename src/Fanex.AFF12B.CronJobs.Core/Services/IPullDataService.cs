namespace Fanex.AFF12B.CronJobs.Core.Services
{
    using Fanex.AFF12B.CronJobs.Core.Models.DTO;
    using Models.MetaModels;
    using Newtonsoft.Json.Linq;

    public interface IPullDataService
    {
        Task<IEnumerable<string>> GetRawDataListAsync(ThirdPartyApiParam param);

        Task<IEnumerable<JObject>> GetUnescapedDataByTimeRangeAsync(ThirdPartyApiParam param);

        Task<IEnumerable<ThirdPartyProductInfo>> GetJsonDataListAsync(ThirdPartyApiParam param);
    }
}
