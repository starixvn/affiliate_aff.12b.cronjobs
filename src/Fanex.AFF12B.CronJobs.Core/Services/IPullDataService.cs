namespace Fanex.AFF12B.CronJobs.Core.Services
{
    using Fanex.AFF12B.CronJobs.Core.Models.DTO;
    using Models.MetaModels;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPullDataService
    {
        Task<IEnumerable<string>> GetRawDataListAsync(ThirdPartyApiParam param);

        Task<(IEnumerable<JObject>, bool)> GetUnescapedDataByTimeRangeAsync(ThirdPartyApiParam param);

        Task<IEnumerable<ThirdPartyProductInfo>> GetJsonDataListAsync(ThirdPartyApiParam param);
    }
}
