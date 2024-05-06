using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Fanex.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute;
using System.Net;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Middlewares
{
    public class ErrorHandlerTest
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _mockNext;

        public ErrorHandlerTest()
        {
            _logger = Substitute.For<ILogger>();
            _mockNext = Substitute.For<RequestDelegate>();
        }

        [Fact]
        public async Task Invoke_WhenCallWithNoException_NotLogging()
        {
            //Arrange
            var errorHandler = new ErrorHandler(_logger, _mockNext);

            //Action
            await errorHandler.Invoke(Substitute.For<HttpContext>());

            // Assert
            await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
            _logger.DidNotReceive().Error(Arg.Any<string>());
        }

        [Fact]
        public async Task Invoke_WhenCallInvokeAndHasException_ErrorHandlerHandleExceptionAndCallNextInvoke()
        {
            //Arrange
            var context = Substitute.For<HttpContext>();
            var reponse = Substitute.For<HttpResponse>();
            var features = Substitute.For<IFeatureCollection>();
            _mockNext.Invoke(Substitute.For<HttpContext>()).Returns(Task.CompletedTask);

            using (var stream = new MemoryStream())
            {
                context.Response.ReturnsForAnyArgs(reponse);
                context.Features.ReturnsForAnyArgs(features);
                reponse.HttpContext.ReturnsForAnyArgs(context);
                reponse.Headers.ReturnsForAnyArgs(new HeaderDictionary());
                reponse.Body = stream;

                features[typeof(IExceptionHandlerFeature)].ReturnsForAnyArgs(new ExceptionHandlerFeature { Error = new Exception("Test") });
                var errorHandler = new ErrorHandler(_logger, _mockNext);

                //Action
                var ex = await Record.ExceptionAsync(() => errorHandler.Invoke(context));

                //Assert
                //await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
                Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            }
        }

        [Fact]
        public async Task Invoke_WhenCallNextInvokeAndThrowException_ErrorHandlerHandleException()
        {
            //Arrange
            var context = Substitute.For<HttpContext>();
            var reponse = Substitute.For<HttpResponse>();
            var features = Substitute.For<IFeatureCollection>();

            using (var stream = new MemoryStream())
            {
                context.Response.ReturnsForAnyArgs(reponse);
                context.Features.ReturnsForAnyArgs(features);
                reponse.HttpContext.ReturnsForAnyArgs(context);
                reponse.Headers.ReturnsForAnyArgs(new HeaderDictionary());
                reponse.Body = stream;

                features.Get<IHttpResponseFeature>().ReturnsForAnyArgs(new HttpResponseFeature());
                _mockNext.Invoke(context).ReturnsForAnyArgs(x => throw new Exception("Test"));
                var errorHandler = new ErrorHandler(_logger, _mockNext);

                //Action
                var ex = await Record.ExceptionAsync(() => errorHandler.Invoke(context));

                //Assert
                await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
                Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            }
        }
    }
}
