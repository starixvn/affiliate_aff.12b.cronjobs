using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Middlewares
{
    public class HangfireMiddlewareTest
    {
        private readonly IServiceCollection _mockServiceCollection;

        public HangfireMiddlewareTest()
        {
            _mockServiceCollection = Substitute.For<IServiceCollection>();
        }

        [Fact]
        public void RegisterHangfire_WhenCall_NotThrowException()
        {
            //Arrange
            var ex = Record.Exception(() => HangfireMiddleware.RegisterHangfire(_mockServiceCollection));

            //Assert
            Assert.Null(ex);
        }
    }
}
