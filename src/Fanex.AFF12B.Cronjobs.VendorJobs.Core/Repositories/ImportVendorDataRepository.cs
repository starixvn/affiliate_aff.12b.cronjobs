using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.Commands;
using Fanex.Data.Repository;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories
{
    public class ImportVendorDataRepository : IImportVendorDataRepository
    {
        private readonly IDynamicRepository _dynamicRepository;
        public ImportVendorDataRepository(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        public async Task ImportDataAsync(int productId, string xmlData)
        {
            await _dynamicRepository.ExecuteAsync(new ImportDataCommand(productId, xmlData));
        }
    }
}
