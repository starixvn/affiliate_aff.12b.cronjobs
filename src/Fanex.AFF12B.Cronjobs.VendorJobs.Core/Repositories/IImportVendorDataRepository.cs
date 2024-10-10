using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories
{
    public interface IImportVendorDataRepository
    {
        Task ImportDataAsync(int productId, string xmlData);
    }
}
