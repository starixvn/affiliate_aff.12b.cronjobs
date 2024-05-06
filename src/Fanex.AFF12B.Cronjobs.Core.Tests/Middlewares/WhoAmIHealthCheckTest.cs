using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using System.Net;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Middlewares
{
    public class WhoAmIHealthCheckTest
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private IWebHostEnvironment WebHostEnvironment { get; set; }

        public WhoAmIHealthCheckTest()
        {
            HttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            WebHostEnvironment = Substitute.For<IWebHostEnvironment>();
        }

        [Fact]
        public async Task CheckHealthAsync_WhenCall_ReturnHealthy()
        {
            //Arrange
            var healthCheck = new WhoAmIHealthCheck(HttpContextAccessor, WebHostEnvironment);
            HttpContextAccessor.HttpContext?.Connection.LocalIpAddress.Returns(new IPAddress(100));
            HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress.Returns(new IPAddress(100));

            //Action
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            //Assert
            Assert.Equal(HealthStatus.Healthy, result.Status);
        }
    }
}
