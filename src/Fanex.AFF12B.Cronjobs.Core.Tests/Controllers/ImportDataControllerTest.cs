using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Controllers;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Controllers
{
    public class ImportDataControllerTest
    {
        private readonly IAppSettings _appSettings;
        private readonly IImportDataService _importDataService;

        public ImportDataControllerTest()
        {
            _appSettings = Substitute.For<IAppSettings>();
            _importDataService = Substitute.For<IImportDataService>();
        }

        [Fact]
        public async Task GetTimeTrackerAsync_WhenCallWith_InvalidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new ImportDataController(_appSettings, _importDataService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = await controller.GetTimeTrackerAsync(new BaseParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetTimeTrackerAsync_WhenCallWith_ValidModel_ReturnCorrect()
        {
            //Arrange
            _importDataService.GetTimeTrackerAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(new DateTime(2023, 06, 01));
            var controller = new ImportDataController(_appSettings, _importDataService);

            //Action
            var result = await controller.GetTimeTrackerAsync(new BaseParam()) as ObjectResult;

            //Assert
            Assert.Contains("Time Tracker", result.Value as string);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task UpdateTimeTrackerAsync_WhenCallWith_InvalidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new ImportDataController(_appSettings, _importDataService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = await controller.UpdateTimeTrackerAsync(new TrackerParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task UpdateTimeTrackerAsync_WhenCallWith_ValidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new ImportDataController(_appSettings, _importDataService);

            //Action
            var result = await controller.UpdateTimeTrackerAsync(new TrackerParam()) as ObjectResult;

            //Assert
            Assert.Contains("updated time tracker", result.Value as string);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task ImportDataToDbAsync_WhenCallWith_InvalidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new ImportDataController(_appSettings, _importDataService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = await controller.ImportDataToDbAsync(new ThirdPartyApiParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task ImportDataToDbAsync_WhenCallWith_ValidModel_ReturnCorrect()
        {
            //Arrange
            var message = "Import successfully!";
            var param = new ThirdPartyApiParam();
            _importDataService.ImportDataToDbAsync<Ticket>(Arg.Any<ThirdPartyApiParam>(), false).Returns(message);
            var controller = new ImportDataController(_appSettings, _importDataService);

            //Action
            var result = await controller.ImportDataToDbAsync(param) as JsonResult;

            //Assert
            Assert.Equal(message, result.Value as string);
        }

        [Fact]
        public async Task ImportConvertedTransactionIdDataToDbAsync_WhenCallWith_InvalidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new ImportDataController(_appSettings, _importDataService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = await controller.ImportConvertedTransactionIdDataToDbAsync(new ThirdPartyApiParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task ImportConvertedTransactionIdDataToDbAsync_WhenCallWith_ValidModel_ReturnCorrect()
        {
            //Arrange
            var message = "Import Converted TransactionId Data successfully!";
            var param = new ThirdPartyApiParam();
            _importDataService.ImportDataToDbAsync<ConvertedTransactionIdTicket>(Arg.Any<ThirdPartyApiParam>(), false).Returns(message);
            var controller = new ImportDataController(_appSettings, _importDataService);

            //Action
            var result = await controller.ImportConvertedTransactionIdDataToDbAsync(param) as JsonResult;

            //Assert
            Assert.Equal(message, result.Value as string);
        }

        [Fact]
        public async Task ImportAdjustedDateTimeTicketDataToDbAsync_WhenCallWith_InvalidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new ImportDataController(_appSettings, _importDataService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = await controller.ImportAdjustedDateTimeTicketDataToDbAsync(new ThirdPartyApiParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task ImportAdjustedDateTimeTicketDataToDbAsync_WhenCallWith_ValidModel_ReturnCorrect()
        {
            //Arrange
            var message = "Import Converted Adjusted DateTime Ticket Data successfully!";
            var param = new ThirdPartyApiParam();
            _importDataService.ImportDataToDbAsync<AdjustedDateTimeTicket>(Arg.Any<ThirdPartyApiParam>(), false).Returns(message);
            var controller = new ImportDataController(_appSettings, _importDataService);

            //Action
            var result = await controller.ImportAdjustedDateTimeTicketDataToDbAsync(param) as JsonResult;

            //Assert
            Assert.Equal(message, result.Value as string);
        }
    }
}
