using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using ONEbook.UM.Models;
using ONEbook.UM.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Middlewares
{
    public class UnderMaintenanceHandlerTest
    {
        private HttpContext _mockHttpContext { get; set; }
        private RequestDelegate _mockNext { get; set; }
        private IMaintenanceService _mockMaintenanceService { get; set; }

        public UnderMaintenanceHandlerTest()
        {
            _mockHttpContext = Substitute.For<HttpContext>();
            _mockNext = Substitute.For<RequestDelegate>();
            _mockMaintenanceService = Substitute.For<IMaintenanceService>();
        }

        [Fact]
        public async Task Invoke_WhenCallWithNoUM_CallNextInvoke()
        {
            //Arrange
            _mockMaintenanceService.IsUnderMaintenanceAsync().Returns(false);
            var errorHandler = new UnderMaintenanceHandler(_mockNext, _mockMaintenanceService);

            //Action
            await errorHandler.Invoke(_mockHttpContext);

            // Assert
            await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
            await _mockMaintenanceService.DidNotReceive().GetUnderMaintenanceTimeAsync();
        }

        [Fact]
        public async Task Invoke_WhenCallWithUMAndAjaxRequest_NotCallNextInvoke()
        {
            //Arrange
            var header = Substitute.For<IHeaderDictionary>();
            header.TryGetValue(Arg.Any<string>(), out Arg.Any<StringValues>()).Returns(x => {
                x[1] = new StringValues("XMLHttpRequest");
                return true;
            });

            var request = Substitute.For<HttpRequest>();
            request.Headers.Returns(header);

            _mockHttpContext.Request.Returns(request);

            _mockMaintenanceService.IsUnderMaintenanceAsync().Returns(true);
            _mockMaintenanceService.GetUnderMaintenanceTimeAsync().Returns(new UnderMaintenanceTime());

            var reponse = Substitute.For<HttpResponse>();

            using (var stream = new MemoryStream())
            {
                _mockHttpContext.Response.ReturnsForAnyArgs(reponse);
                reponse.HttpContext.ReturnsForAnyArgs(_mockHttpContext);
                reponse.Body = stream;

                var errorHandler = new UnderMaintenanceHandler(_mockNext, _mockMaintenanceService);

                //Action
                var ex = await Record.ExceptionAsync(() => errorHandler.Invoke(_mockHttpContext));

                // Assert
                await _mockNext.DidNotReceive().Invoke(Arg.Any<HttpContext>());
            }
        }

        [Fact]
        public async Task Invoke_WhenCallWithUMAndNotAjaxRequest_NotCallNextInvoke()
        {
            //Arrange
            var header = Substitute.For<IHeaderDictionary>();
            header.TryGetValue(Arg.Any<string>(), out Arg.Any<StringValues>()).Returns(false);

            var request = Substitute.For<HttpRequest>();
            request.Headers.Returns(header);

            _mockHttpContext.Request.Returns(request);

            _mockMaintenanceService.IsUnderMaintenanceAsync().Returns(true);
            _mockMaintenanceService.GetUnderMaintenanceTimeAsync().Returns(new UnderMaintenanceTime());

            var reponse = Substitute.For<HttpResponse>();

            using (var stream = new MemoryStream())
            {
                _mockHttpContext.Response.ReturnsForAnyArgs(reponse);
                reponse.Body = stream;

                var errorHandler = new UnderMaintenanceHandler(_mockNext, _mockMaintenanceService);

                //Action
                var ex = await Record.ExceptionAsync(() => errorHandler.Invoke(_mockHttpContext));

                // Assert
                await _mockNext.DidNotReceive().Invoke(Arg.Any<HttpContext>());
            }
        }
    }
}
