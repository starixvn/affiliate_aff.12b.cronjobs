using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Repositories
{
    public interface IImportDataRepository
    {
        Task ImportDataAsync(int productId, string xmlData, byte jobType = 0);
    }
}
