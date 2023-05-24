using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.UnitTest.Middlewares
{
    public class MiddlewareExtensionsTest
    {
        private readonly IServiceCollection _mockServiceCollection;
        private readonly IConfiguration _mockConfiguration;
        private readonly IApplicationBuilder _mockApplicationBuilder;

        public MiddlewareExtensionsTest()
        {
            _mockServiceCollection = Substitute.For<IServiceCollection>();
            _mockConfiguration = Substitute.For<IConfiguration>();
            _mockApplicationBuilder = Substitute.For<IApplicationBuilder>();
        }

        [Fact]
        public void UsePingCheck_WhenCall_NotThrowException()
        {
            //Arrange
            var ex = Record.Exception(() => MiddlewareExtensions.UsePingCheck(_mockApplicationBuilder));

            //Assert
            Assert.Null(ex);
        }

        [Fact]
        public void UseJWTokenOnPage_WhenCall_NotThrowException()
        {
            //Arrange
            var ex = Record.Exception(() => MiddlewareExtensions.UseJWTokenOnPage(_mockApplicationBuilder));

            //Assert
            Assert.Null(ex);
        }

        [Fact]
        public void UseErrorHandler_WhenCall_NotThrowException()
        {
            //Arrange
            var ex = Record.Exception(() => MiddlewareExtensions.UseErrorHandler(_mockApplicationBuilder));

            //Assert
            Assert.Null(ex);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UseStaticResource_WhenCall_NotThrowException(bool isDev)
        {
            //Arrange
            var ex = Record.Exception(() => MiddlewareExtensions.UseStaticResource(_mockApplicationBuilder, isDev));

            //Assert
            Assert.Null(ex);
        }

        [Fact]
        public void AddUnderMaintenance_WhenCall_NotThrowException()
        {
            //Arrange
            var ex = Record.Exception(() => MiddlewareExtensions.AddUnderMaintenance(_mockServiceCollection, _mockConfiguration));

            //Assert
            Assert.Null(ex);
        }

        [Fact]
        public void UseUnderMaintenance_WhenCall_NotThrowException()
        {
            //Arrange
            var ex = Record.Exception(() => MiddlewareExtensions.UseUnderMaintenance(_mockApplicationBuilder));

            //Assert
            Assert.Null(ex);
        }
    }
}
