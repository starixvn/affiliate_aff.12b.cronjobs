using Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.Commands;
using Fanex.Data.Repository;
using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Repositories
{
    public class ImportDataRepository : IImportDataRepository
    {
        private readonly IDynamicRepository _dynamicRepository;
        public ImportDataRepository(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        public async Task ImportDataAsync(int productId, string xmlData, byte jobType = 0)
        {
            await _dynamicRepository.ExecuteAsync(new ImportDataCommand(productId, xmlData, jobType));
        }
    }
}
