using Fanex.AFF12B.CronJobs.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.UnitTest.Controllers
{
    public class ErrorControllerTest
    {
        [Fact]
        public void Index_WhenCallWith_UnauthorizedCode_ReturnCorrect()
        {
            //Arrange
            var controller = new ErrorController();

            //Action
            var result = controller.Index(401) as ObjectResult;

            //Assert
            Assert.Equal("You don't have permission to access", result.Value);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public void Index_WhenCallWith_AnotherStatusCode_ReturnCorrect()
        {
            //Arrange
            var controller = new ErrorController();

            //Action
            var result = controller.Index(0) as ObjectResult;

            //Assert
            Assert.Equal("Page not found", result.Value);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
