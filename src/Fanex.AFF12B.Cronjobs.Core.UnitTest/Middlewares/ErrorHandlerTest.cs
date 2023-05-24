using Fanex.AFF12B.Cronjobs.Core.UnitTest.Helpers;
using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.UnitTest.Middlewares
{
    public class ErrorHandlerTest
    {
        private RequestDelegate _mockNext { get; set; }

        public ErrorHandlerTest()
        {
            _mockNext = Substitute.For<RequestDelegate>();
        }

        [Fact]
        public async Task Invoke_WhenCallWithNoException_NotLogging()
        {
            //Arrange
            MockingHelper.SetUpLogEngineEnviroment();
            var errorHandler = new ErrorHandler(_mockNext);

            //Action
            await errorHandler.Invoke(Substitute.For<HttpContext>());

            // Assert
            await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
        }

        [Fact]
        public async Task Invoke_WhenCallInvokeAndHasException_ErrorHandlerHandleExceptionAndCallNextInvoke()
        {
            //Arrange
            MockingHelper.SetUpLogEngineEnviroment();
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
                var errorHandler = new ErrorHandler(_mockNext);

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
            MockingHelper.SetUpLogEngineEnviroment();
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
                var errorHandler = new ErrorHandler(_mockNext);

                //Action
                var ex = await Record.ExceptionAsync(() => errorHandler.Invoke(context));

                //Assert
                await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
                Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            }
        }
    }
}
