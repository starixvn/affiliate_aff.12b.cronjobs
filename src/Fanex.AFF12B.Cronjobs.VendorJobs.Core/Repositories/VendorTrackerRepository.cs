using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.Commands;
using Fanex.Data.Repository;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories
{
    public class VendorTrackerRepository : IVendorTrackerRepository
    {
        private readonly IDynamicRepository _dynamicRepository;
        public VendorTrackerRepository(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        public async Task UpdateTrackerAsync(UpTrackerCommand command)
        {
            await _dynamicRepository.ExecuteAsync(command);
        }

        public async Task<DateTime?> GetTrackerAsync(TrackerCriteria criteria)
        {
            return await _dynamicRepository.GetAsync<DateTime?>(criteria);
        }
    }
}
