using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    [Headers("X-Token: XE5JL1akYFnGlghllSRXqs8F3ZT2bgeZQnJ1QchJ0knuuq8K68V4OtAhqqw-wi_UQc4h9Uf59EOMEKyzek_gPsavrS6NHnhJs64G9njjrXMsfdnJhXmWiCW0RJbY5RhO")]
    public interface IBetRadarApi
    {
        [Get("/player/trans/thirdparty/settlementfeed")]
        Task<string> GetRawDataAsync(BetRadarParam param);
    }
}
