using Fanex.AFF12B.Cronjobs.Core.Tests.Helpers;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.RecuringTasks;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Services;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using ONEbook.UM.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.RecuringTasks
{
    public class FetchThirdPartyProductDataTaskTest
    {
        private readonly ILogger _logger;
        private IConfiguration _configuration;
        private readonly IImportDataService _importDataService;
        private readonly IMaintenanceService _maintenanceService;

        public FetchThirdPartyProductDataTaskTest()
        {
            _logger = Substitute.For<ILogger>();
            _configuration = Substitute.For<IConfiguration>();
            _importDataService = Substitute.For<IImportDataService>();
            _maintenanceService = Substitute.For<IMaintenanceService>();
        }

        [Fact]
        public async Task FetchDataAsync_WhenCallWithUnderMaintenance_ShouldCallLogger()
        {
            //Arrange
            _maintenanceService.IsUnderMaintenance().Returns(true);
            var task = CreateFetchThirdPartyProductDataTask();

            //Action
            await task.FetchDataAsync(string.Empty, new ThirdPartyProductSetting());

            //Assert
            _logger.Received().Info("System is currently unavailable due to scheduled maintenance!");
        }

        [Fact]
        public async Task FetchDataAsync_WhenCallWithNotUnderMaintenance_ShouldCallLogger()
        {
            //Arrange
            _configuration = MockingHelper.SetUpConfiguration("AppSettings:ApiServerTimeOffset", "0");
            _importDataService.GetTimeTrackerAsync(Arg.Any<int>()).Returns(new DateTime(2023, 06, 06));
            _maintenanceService.IsUnderMaintenance().Returns(false);
            var task = CreateFetchThirdPartyProductDataTask();

            //Action
            await task.FetchDataAsync(string.Empty, new ThirdPartyProductSetting());

            //Assert
            _logger.DidNotReceive().Info("System is currently unavailable due to scheduled maintenance!");
            await _importDataService.Received().ImportDataToDbAsync<Ticket>(Arg.Any<ThirdPartyApiParam>(), true);
        }

        private FetchThirdPartyProductDataTask CreateFetchThirdPartyProductDataTask()
            => new FetchThirdPartyProductDataTask(_logger, _configuration, _importDataService, _maintenanceService);
    }
}
