using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Repositories;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Services;
using Fanex.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Services
{
    public class ImportDataServiceTest
    {
        private readonly ILogger _logger;
        private readonly IQueueRepository _queueRepository;
        private readonly ITrackerRepository _trackerRepository;
        private readonly IPullDataService _pullDataService;

        public ImportDataServiceTest()
        {
            _logger = Substitute.For<ILogger>();
            _queueRepository = Substitute.For<IQueueRepository>();
            _trackerRepository = Substitute.For<ITrackerRepository>();
            _pullDataService = Substitute.For<IPullDataService>();
        }

        [Fact]
        public async Task GetTimeTrackerAsync_WhenCallWithHasTimeTracker_ReturnCorrectData()
        {
            //Arrange
            var timeTacker = new DateTime(2023, 06, 10, 00, 00, 00, 00);
            _trackerRepository.GetTrackerAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(Task.FromResult<DateTime?>(timeTacker));
            var service = CreateImportDataService();

            //Action
            var result = await service.GetTimeTrackerAsync(123);

            //Assert
            Assert.Equal(timeTacker, result);
        }

        [Fact]
        public async Task GetTimeTrackerAsync_WhenCallWithNoHasTimeTracker_ReturnFirstDayOfMonth()
        {
            //Arrange
            _trackerRepository.GetTrackerAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(Task.FromResult<DateTime?>(null));
            var service = CreateImportDataService();

            //Action
            var result = await service.GetTimeTrackerAsync(123);

            //Assert
            Assert.Equal(1, result.Day);
        }

        [Fact]
        public async Task UpdateTimeTrackerAsync_WhenCall_ShouldCallRepository()
        {
            //Arrange
            var service = CreateImportDataService();

            //Action
            await service.UpdateTimeTrackerAsync(new TrackerParam());

            //Assert
            await _trackerRepository.Received().AddTrackerAsync(Arg.Any<TrackerParam>());
            _logger.Received().Info(Arg.Any<string>());
        }

        [Fact]
        public async Task ImportDataToDbAsync_WhenCallWithIsRunningJob_ShouldCallRepositoryAddTracker()
        {
            //Arrange
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 00, 30, 00),
                TpType = 1,
                MaxMinutesRange = 30
            };
            _pullDataService.GetJsonDataListAsync<Ticket>(Arg.Any<ThirdPartyApiParam>()).Returns(new List<ThirdPartyProductJson<Ticket>> { new ThirdPartyProductJson<Ticket> { BetTransTicket = new List<Ticket>() } });
            var service = CreateImportDataService();

            //Action
            await service.ImportDataToDbAsync<Ticket>(param, true);

            //Assert
            await _queueRepository.Received().AddQueueAsync(Arg.Any<QueueParam>());
            await _trackerRepository.Received().AddTrackerAsync(Arg.Any<TrackerParam>());
            _logger.Received(2).Info(Arg.Any<string>());
            await _logger.DidNotReceive().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task ImportDataToDbAsync_WhenCallWithIsNotRunningJob_ShouldNotCallRepositoryAddTracker()
        {
            //Arrange
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 00, 30, 00),
                TpType = 1,
                MaxMinutesRange = 30
            };
            _pullDataService.GetJsonDataListAsync<Ticket>(Arg.Any<ThirdPartyApiParam>()).Returns(new List<ThirdPartyProductJson<Ticket>> { new ThirdPartyProductJson<Ticket> { BetTransTicket = new List<Ticket>() } });
            var service = CreateImportDataService();

            //Action
            await service.ImportDataToDbAsync<Ticket>(param, false);

            //Assert
            await _queueRepository.Received().AddQueueAsync(Arg.Any<QueueParam>());
            await _trackerRepository.DidNotReceive().AddTrackerAsync(Arg.Any<TrackerParam>());
            _logger.Received(3).Info(Arg.Any<string>());
            await _logger.DidNotReceive().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task ImportDataToDbAsync_WhenCallWithHasExceptionTimeRange_ShouldNotCallRepositoryAddQueue()
        {
            //Arrange
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 00, 45, 00),
                TpType = 1,
                MaxMinutesRange = 0
            };
            var service = CreateImportDataService();

            //Action
            await service.ImportDataToDbAsync<Ticket>(param, false);

            //Assert
            await _queueRepository.DidNotReceive().AddQueueAsync(Arg.Any<QueueParam>());
            await _trackerRepository.DidNotReceive().AddTrackerAsync(Arg.Any<TrackerParam>());
            _logger.Received().Info(Arg.Any<string>());
            await _logger.Received().ErrorAsync(Arg.Any<string>());
        }

        public ImportDataService CreateImportDataService() => new ImportDataService(_logger, _queueRepository, _trackerRepository, _pullDataService);
    }
}
