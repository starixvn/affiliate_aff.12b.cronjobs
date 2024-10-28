using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.Core.Tests.Helpers;
using Fanex.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Fanex.AFF12B.CronJobs.Core.Tests.Attributes
{
    public class IPFilterAttributeTest
    {
        private readonly ILogger _logger;
        private readonly IOptions<ApplicationOptions> _options;
        private readonly ActionExecutingContext _actionExecutingContext;
        private readonly HttpContext _mockHttpContext;

        public IPFilterAttributeTest()
        {
            _logger = Substitute.For<ILogger>();
            _mockHttpContext = Substitute.For<HttpContext>();
            _options = Substitute.For<IOptions<ApplicationOptions>>();

            var actionContext = new ActionContext(
                _mockHttpContext,
                Substitute.For<RouteData>(),
                Substitute.For<ActionDescriptor>(),
                new ModelStateDictionary()
            );

            _actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Substitute.For<Controller>()
            );

            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Host.Returns(HostString.FromUriComponent("abcxyz.com"));

            _mockHttpContext.Request.Returns(httpRequest);
        }

        [Fact]
        public void OnActionExecuting_WhenFromInternal_NoChangeResult()
        {
            //Arrange
            var fakeContent = "No_Change";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string>() });

            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Host.Returns(HostString.FromUriComponent(""));

            _mockHttpContext.Request.Returns(httpRequest);

            //Action
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }


        [Fact]
        public void OnActionExecuting_WhenEmptyWhiteListIP_NoChangeResult()
        {
            //Arrange
            var fakeContent = "No_Change";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string>() });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(new IPAddress(0x2414188f));
            _mockHttpContext.Connection.Returns(connectionInfo);

            //Action
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_WhenAllowIP_WhiteListSingle_NoChangeResult()
        {
            //Arrange
            const int requestIP = 0x2414188f;
            const string WhitelistIP = "143.24.20.36";

            const string fakeContent = "NOCHANGE";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { WhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(new IPAddress(requestIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            //Action
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_WhenNotAllowIP_WhiteListSingle_ReturnResultWithDeniedMessage()
        {
            //Arrange
            const long requestIPNumber = 1532995706;
            const string requestIP = "122.168.95.91";
            const string WhitelistIP = "172.168.95.121";

            var expectedResult = $"The IP Address: {requestIP} was blocked";
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { WhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(new IPAddress(requestIPNumber));

            _mockHttpContext.Connection.Returns(connectionInfo);


            //Action
            var filter = new IPFilterAttribute(_logger, _options);

            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            Assert.Equal(expectedResult, actualResult.Content);
        }

        [Theory]
        [InlineData("122.168.95.121")]
        [InlineData("122.168.95.122")]
        [InlineData("122.168.95.123")]
        [InlineData("122.168.95.124")]
        [InlineData("122.168.95.125")]
        [InlineData("122.168.95.126")]
        [InlineData("122.168.95.127")]
        public void OnActionExecuting_WhenAllowIP_WhiteListRange_NoChangeResult(string requestIP)
        {
            //Arrange
            const string WhitelistIP = "122.168.95.121/29";

            const string fakeContent = "NOCHANGE";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { WhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(requestIP));
            _mockHttpContext.Connection.Returns(connectionInfo);
            //Action
            var filter = new IPFilterAttribute(_logger, _options);

            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Theory]
        [InlineData("122.168.95.119")]
        [InlineData("122.168.95.128")]
        public void OnActionExecuting_WhenNotAllowIP_WhiteListRange_ReturnResultWithDeniedMessage(string requestIP)
        {
            //Arrange
            const string WhitelistIP = "122.168.95.121/29";

            var expectedResult = $"The IP Address: {requestIP} was blocked";
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { WhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(requestIP));

            _mockHttpContext.Connection.Returns(connectionInfo);


            //Action
            var filter = new IPFilterAttribute(_logger, _options);

            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            Assert.Equal(expectedResult, actualResult.Content);
        }

        [Theory]
        [InlineData("122.168.95.256")]
        [InlineData("122.168.95.121/100")]
        public void OnActionExecuting_WhiteListIPInvalid_ThrowExceptionWithMessage(string whiteListIP)
        {
            //Arrange
            MockingHelper.SetUpLogEngineEnviroment();
            const string requestIP = "122.168.95.121";

            var expectedResult = $"The IP Address: {requestIP} was blocked";
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { whiteListIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(requestIP));

            _mockHttpContext.Connection.Returns(connectionInfo);

            //Action
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            Assert.Equal(expectedResult, actualResult.Content);
        }
    }
}
