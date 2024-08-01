using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    public interface ICrystalGpiApi
    {
        [Post("/REST/ApiReport/GameTransactionDetail")]
        Task<string> GetRawDataAsync([Body(BodySerializationMethod.Serialized)] CrystalGpiParam param);
    }
}
