using Fanex.Athena.AFF12B.CronJobs.Core.Contants;
using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.Athena.AFF12B.CronJobs.Core.Repositories;
using Fanex.Athena.AFF12B.CronJobs.Core.Helpers;
using Newtonsoft.Json;
using System.Text;
using Fanex.Athena.AFF12B.CronJobs.Core.Constant;
using Fanex.Logging;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Services
{
    public class ImportDataService : IImportDataService
    {
        private readonly IQueueRepository _queueRepository;
        private readonly ITrackerRepository _trackerRepository;
        private readonly IPullDataService _pullDataService;

        public ImportDataService(
            IQueueRepository queueRepository,
            ITrackerRepository trackerRepository,
            IPullDataService pullDataService)
        {
            _queueRepository = queueRepository;
            _trackerRepository = trackerRepository;
            _pullDataService = pullDataService;
        }

        public async Task<DateTime> GetTimeTrackerAsync(int tpType, string subItem = DefaultValue.String)
        {
            return await _trackerRepository.GetTracker(tpType, subItem);
        }

        public async Task<string> ImportDataAndTrackTimeToDbAsync(ThirdPartyApiParam param)
        {
            var logBuilder = new StringBuilder();
            var message = $"import data for: TpType={param.TpType}, SubItem={param.SubItem} and from {param.BeginDate.ToString(Format.DateTimeFormat)} to {param.EndDate.ToString(Format.DateTimeFormat)}";

            try
            {
                var pullRange = TimeSpan.FromMinutes(param.MaxMinutesRange);
                var timeRanges = DateTimeHelper.GetProportionTimeRange(param.BeginDate, param.EndDate, pullRange);

                Logger.Log.Info($"Start {message}");

                foreach (var range in timeRanges)
                {

                    param.BeginDate = range.Item1;
                    param.EndDate = range.Item2;

                    await ImportDataAndTrackTimeByRangeToDbAsync(param);
                }

                var logMessage = $"Finished {message}";
                Logger.Log.Info(logMessage);

                return logMessage;
            }
            catch (Exception ex)
            {
                logBuilder.AppendLine($"Throw exception when {message}");
                logBuilder.AppendLine(ErrorMessageHelper.GenerateMessage(ex));

                var logMessage = logBuilder.ToString();

                Logger.Log.Error(logMessage);

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
