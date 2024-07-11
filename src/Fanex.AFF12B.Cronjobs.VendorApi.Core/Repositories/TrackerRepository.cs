using Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.Commands;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels;
using Fanex.Data.Repository;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Repositories
{
    public class TrackerRepository : ITrackerRepository
    {
        private readonly IDynamicRepository _dynamicRepository;
        public TrackerRepository(IDynamicRepository dynamicRepository)
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
