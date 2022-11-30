using Fanex.Athena.AFF12B.CronJobs.Core.Configurations;
using Fanex.Athena.AFF12B.CronJobs.Core.Contants;
using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.Athena.AFF12B.CronJobs.Core.Repositories;
using Fanex.Athena.AFF12B.CronJobs.Core.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Services
{
    public class ImportDataService : IImportDataService
    {
        private readonly ILogger<ImportDataService> _logger;
        private readonly IAppSettings _appSettings;
        private readonly IQueueRepository _queueRepository;
        private readonly ITrackerRepository _trackerRepository;
        private readonly IPullDataService _pullDataService;

        public ImportDataService(
            ILogger<ImportDataService> logger,
            IAppSettings appSettings,
            IQueueRepository queueRepository,
            ITrackerRepository trackerRepository,
            IPullDataService pullDataService)
        {
            _logger = logger;
            _appSettings = appSettings;
            _queueRepository = queueRepository;
            _trackerRepository = trackerRepository;
            _pullDataService = pullDataService;
        }

        public async Task<string> ImportDataAndTrackTimeToDbAsync(ThirdPartyApiParam param)
        {
            var logBuilder = new StringBuilder();
            var message = $"import data for: TpType={param.TpType}, SubItem={param.SubItem} and from {param.BeginDate.ToString(DateTimeFormats.Request)} to {param.EndDate.ToString(DateTimeFormats.Request)}";

            try
            {
                var pullRange = TimeSpan.FromMinutes(_appSettings.MaxTimeRange);
                var timeRanges = DateTimeHelper.GetProportionTimeRange(param.BeginDate, param.EndDate, pullRange);

                _logger.LogInformation($"Start {message}");

                foreach (var range in timeRanges)
                {

                    param.BeginDate = range.Item1;
                    param.EndDate = range.Item2;

                    await ImportDataAndTrackTimeByRangeToDbAsync(param);
                }

                var logMessage = $"Finished {message}";
                _logger.LogInformation(logMessage);

                return logMessage;
            }
            catch (Exception ex)
            {
                logBuilder.AppendLine($"Throw exception when {message}");
                logBuilder.AppendLine(LogHelper.BuildErrorMessage(ex));

                var logMessage = logBuilder.ToString();

                _logger.LogError(logMessage);

                throw;
            }
        }

        private async Task ImportDataAndTrackTimeByRangeToDbAsync(ThirdPartyApiParam param)
        {
            var dataList = await _pullDataService.GetUnescapedAndRemoveDataInfoDataByTimeRangeAsync(param);
            var queueParam = new QueueParam
            {
                TpType = param.TpType,
                SubItem = param.SubItem
            };

            foreach (var data in dataList)
            {
                queueParam.JsonData = JsonConvert.SerializeObject(data);

                await _queueRepository.AddQueue(queueParam);
            }

            var trackerParam = new TrackerParam
            {
                TpType = param.TpType,
                SubItem = param.SubItem,
                CallTime = param.EndDate
            };
            await _trackerRepository.AddTracker(trackerParam);
        }
    }
}
