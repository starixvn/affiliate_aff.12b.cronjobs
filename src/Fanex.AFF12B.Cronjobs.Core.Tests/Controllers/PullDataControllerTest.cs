using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Controllers;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Controllers
{
    public class PullDataControllerTest
    {
        private readonly IAppSettings _appSettings;
        private readonly IPullDataService _pullDataService;

        public PullDataControllerTest()
        {
            _appSettings = Substitute.For<IAppSettings>();
            _pullDataService = Substitute.For<IPullDataService>();
        }

        [Fact]
        public async Task GetRawDataListAsync_WhenCallWith_InvalidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new PullDataController(_appSettings, _pullDataService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = await controller.GetRawDataListAsync(new ThirdPartyApiParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetRawDataListAsync_WhenCallWith_ValidModel_ReturnCorrect()
        {
            //Arrange
            var maxMinutesRange = 30;
            var param = new ThirdPartyApiParam();
            _appSettings.MaxMinutesRange.Returns(maxMinutesRange);
            _pullDataService.GetRawDataListAsync(Arg.Any<ThirdPartyApiParam>()).Returns(new List<ThirdPartyProductRaw> { new ThirdPartyProductRaw() });
            var controller = new PullDataController(_appSettings, _pullDataService);

            //Action
            var result = await controller.GetRawDataListAsync(param) as JsonResult;

            //Assert
            Assert.NotEmpty(result.Value as List<ThirdPartyProductRaw>);
            Assert.Equal(maxMinutesRange, param.MaxMinutesRange);
        }

        [Fact]
        public async Task GetJsonDataListAsync_WhenCallWith_InvalidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new PullDataController(_appSettings, _pullDataService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = await controller.GetJsonDataListAsync(new ThirdPartyApiParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetJsonDataListAsync_WhenCallWith_ValidModel_ReturnCorrect()
        {
            //Arrange
            var maxMinutesRange = 30;
            var param = new ThirdPartyApiParam { MaxMinutesRange = 15 };
            _appSettings.MaxMinutesRange.Returns(maxMinutesRange);
            _pullDataService.GetJsonDataListAsync<Ticket>(Arg.Any<ThirdPartyApiParam>()).Returns(new List<ThirdPartyProductJson<Ticket>> { new ThirdPartyProductJson<Ticket>() });
            var controller = new PullDataController(_appSettings, _pullDataService);

            //Action
            var result = await controller.GetJsonDataListAsync(param) as JsonResult;

            //Assert
            Assert.NotEmpty(result.Value as List<ThirdPartyProductJson<Ticket>>);
            Assert.NotEqual(maxMinutesRange, param.MaxMinutesRange);
        }
    }
}
