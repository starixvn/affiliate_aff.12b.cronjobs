using Fanex.AFF12B.CronJobs.Core.Controllers;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Net;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Controllers
{
    public class AuthControllerTest
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthControllerTest()
        {
            _authenticateService = Substitute.For<IAuthenticateService>();
        }

        [Fact]
        public void RequestToken_WhenCallWith_InvalidModel_ReturnCorrect()
        {
            //Arrange
            var controller = new AuthController(_authenticateService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = controller.RequestToken(new TokenRequestParam()) as ObjectResult;

            //Assert
            Assert.Equal("Invalid Request", result.Value);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void RequestToken_WhenCallWith_Unauthentication_ReturnCorrect()
        {
            //Arrange
            _authenticateService.IsAuthenticated(Arg.Any<TokenRequestParam>()).Returns(Tuple.Create<bool, Token>(false, null));
            var controller = new AuthController(_authenticateService);

            //Action
            var result = controller.RequestToken(new TokenRequestParam()) as ObjectResult;

            //Assert
            Assert.Equal("Wrong username or password", result.Value);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void RequestToken_WhenCallWith_Authenticated_ReturnCorrect()
        {
            //Arrange
            _authenticateService.IsAuthenticated(Arg.Any<TokenRequestParam>()).Returns(Tuple.Create<bool, Token>(true, new Token()));
            var controller = new AuthController(_authenticateService);

            //Action
            var result = controller.RequestToken(new TokenRequestParam()) as ObjectResult;

            //Assert
            Assert.NotNull(result.Value as Token);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }
    }
}
