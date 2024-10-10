using Fanex.AFF12B.Cronjobs.Core.Tests.Helpers;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Extensions
{
    public class HttpContextExtensionsTest
    {
        private static readonly SecurityKey Security_Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("cf6cd78a9e3d01a175d1604242b2d6ff7f33c51bff6525d1c5917e50b9495b79"));
        private static readonly TokenValidationParameters Token_Validation_Param = GetTokenValidationParameters();

        private static TokenValidationParameters GetTokenValidationParameters() => new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = Security_Key,
            TokenDecryptionKey = Security_Key,
            ValidIssuer = "12B",
            ValidAudience = "12B",
            ValidateIssuer = true,
            ValidateAudience = true,
            RoleClaimType = ClaimsIdentity.DefaultNameClaimType,
            RequireExpirationTime = true,
            ValidateLifetime = true
        };

        [Fact]
        public void Authorize_WhenCallWithIsAuthenticated_ReturnTrue()
        {
            //Arrange
            var httpContext = Substitute.For<HttpContext>();
            httpContext.User.Identity.IsAuthenticated.Returns(true);

            //Action
            var result = httpContext.IsAuthorize(Token_Validation_Param);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Authorize_WhenCallWithQueryContainsKey_ButEmptryAccessToken_ReturnFalse()
        {
            //Arrange
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Query.ContainsKey(Arg.Any<string>()).Returns(true);
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "access_token", StringValues.Empty } }));

            //Action
            var result = httpContext.IsAuthorize(Token_Validation_Param);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Authorize_WhenCallWithQueryNotContainsKey_AndEmptryCookie_ReturnFalse()
        {
            //Arrange
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Query.ContainsKey(Arg.Any<string>()).Returns(false);
            var cookieCollection = Substitute.For<IRequestCookieCollection>();
            httpContext.Request.Cookies.Returns(cookieCollection);

            //Action
            var result = httpContext.IsAuthorize(Token_Validation_Param);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Authorize_WhenCallWithQueryContainsKeyAndValidAccessToken_ReturnTrue()
        {
            //Arrange
            var token = new StringValues("eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwidHlwIjoiSldUIiwiY3R5IjoiSldUIn0..D6PODZn4W0jm2YlvUIZCLA.BJBSYm6gdXrKr-8K72MKaHjF3tSv9_YTd5oh3JWu9z6TKtzfkOS5E27zZeWRVNjaf6IS1XcWwItY4XtVN4GWt18l0NWD-dFWFMrMHFqkTWwg48ZuN1f3HxictIIhfF746cTBxKYouRR8W3Rcmj59-6ERMxNmJCli89y5H1jqUKISfWLXMUKcDCfynlY3ytO37-G1oVcB7TObinw0Qcm44CbyYut2YKqCof3xGlmCrUHO74Q4Z2pMiclXfeGJtg5TMEjMP3sg0Asg7deSivgr0GOFOlaKHTESmQBiC_-5QQk5SvEOo_jswRl24jgXv9jtH6GtCBCkthuvkS1MzsUeuBjYL77yJSfzKic2qrXVJRGnys44EhWgzEdh4h_4Ge-LAa3ktYmGTEJ6Zu7tIP7EDC80OmqqHZGKlhtnyJVld30f0z7kwxVxWQ6Oouojomoh.KIx5NWPTs1CTteZiXewzW-5-N7r0S8zRn8sckSGsP58");
            var validateTokenParam = GetTokenValidationParameters();
            validateTokenParam.ValidateLifetime = false;
            validateTokenParam.RequireExpirationTime = false;

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Query.ContainsKey(Arg.Any<string>()).Returns(true);
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "access_token", token } }));

            //Action
            var result = httpContext.IsAuthorize(validateTokenParam);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Authorize_WhenCallWithQueryContainsKeyAndInvalidAccessToken_ReturnFalse()
        {
            //Arrange
            MockingHelper.SetUpLogEngineEnviroment();
            var token = new StringValues("eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwidHlwIjoiSldUIn0..FwKYBBceQZmD8QuagegNKA.qTCGwzT1n91qeJx2ZMD42yy76SWS12gATbn6pqbu8o1meD2g3Fvt1vqHvRRC8vscJCwfUioTXUEyaTptZadzTKI3BDI728-s5503HDv4o2qYtCR9XISdn9paTN-9VYqLhVNZ-b8qX-Vu-E_ZW0Fs-OHIuERujTsXvGDJRRZ9YuvDryU9nh8YsEnj27-Bhz7DMHoucWY0JoU4iHFVcwMX_2sQ6bNOuCeDYUEhPnert_MwCLgKA12-qmFKT_zgHdqjF4Rm9RDntmNAkZInFuYA-SwjO4akRV1SVxPYtwuqP0lrTCTzvqBSO3HTwRfD5uW4.wmuPkdr0b6no2a_IuzmDUaeqFfnNxWbj60xXnHqBDG4");
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Query.ContainsKey(Arg.Any<string>()).Returns(true);
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "access_token", token } }));

            //Action
            var result = httpContext.IsAuthorize(Token_Validation_Param);

            //Assert
            Assert.False(result);
        }
    }
}
