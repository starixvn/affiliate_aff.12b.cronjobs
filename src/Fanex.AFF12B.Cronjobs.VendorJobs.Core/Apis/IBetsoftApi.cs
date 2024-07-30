using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    //[Headers("Cookie:JSESSIONID=node0m7o0cudoog0tk0ke6ikcn5g01716360.node0")]
    public interface IBetsoftApi
    {
        [Post("/xinji/playersBetHistory.do")]
        Task<string> GetRawDataAsync([Body(BodySerializationMethod.Serialized)] BetsoftParam param);
    }
}
