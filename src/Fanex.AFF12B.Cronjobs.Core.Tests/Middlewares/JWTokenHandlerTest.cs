using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Middlewares
{
    public class JWTokenHandlerTest
    {
        private readonly HttpContext _mockContext;
        private readonly HttpRequest _mockRequest;
        private readonly IQueryCollection _mockQueryCollection;
        private readonly IHeaderDictionary _mockHeader;
        const string Token = "ANYTOKEN";
        public JWTokenHandlerTest()
        {
            _mockContext = Substitute.For<HttpContext>();
            _mockRequest = Substitute.For<HttpRequest>();
            _mockQueryCollection = Substitute.For<IQueryCollection>();
            _mockHeader = Substitute.For<IHeaderDictionary>();
        }

        [Fact]
        public async Task Invoke_WithoutTokenHeader_AddTokenToHeaderAsync()
        {
            //Arrange
            _mockHeader.ContainsKey("Authorization").Returns(false);

            _mockQueryCollection.TryGetValue(Arg.Any<string>(), out Arg.Any<StringValues>()).Returns(x =>
            {
                x[1] = new StringValues(Token);
                return true;
            });

            _mockRequest.Headers.Returns(_mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);

            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            //Action
            await handler.Invoke(_mockContext).ConfigureAwait(false);

            //Assert
            _mockRequest.Headers.Received().Add("Authorization", "Bearer " + Token);
        }


        [Fact]
        public async Task Invoke_InValidTokenInHeader_NotAddTokenToHeaderAsync()
        {
            //Arrange
            var mockHeader = Substitute.For<IHeaderDictionary>();

            mockHeader.ContainsKey("Authorization").Returns(true);
            mockHeader["Authorization"].Returns(StringValues.Empty);
            _mockRequest.Headers.Returns(mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);

            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            //Action
            await handler.Invoke(_mockContext).ConfigureAwait(false);

            //Assert
            _mockRequest.Headers.DidNotReceive().Add("Authorization", "Bearer " + Token);
        }

        [Fact]
        public async Task Invoke_WithoutToken_NotAddTokenToHeaderAsync()
        {
            //Arrange
            _mockRequest.ClearReceivedCalls();
            var mockHeader = Substitute.For<IHeaderDictionary>();

            mockHeader.ContainsKey("Authorization").Returns(false);
            _mockRequest.Headers.Returns(mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);

            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            //Action
            await handler.Invoke(_mockContext).ConfigureAwait(false);

            //Assert
            _mockRequest.Headers.DidNotReceive().Add("Authorization", "Bearer " + Token);
        }
    }
}
