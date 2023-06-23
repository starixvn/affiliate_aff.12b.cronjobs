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
        private static readonly SecurityKey Security_Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("cf6cd78a9e3d01d6ff7f33c51bff6525d1c5917e50b9a175d1604242b2495b79"));
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
            ValidateLifetime = true,
            NameClaimType = ClaimTypes.Email
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
            var token = new StringValues("eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwidHlwIjoiSldUIn0..jvzLI5Jq9Fz9OItsZ_aOJA.O02iE8tWVSvIg67YzWeH2HCghsatfL856dpAvtLNzOLy78YPvVGuRcP6f_487QAHrJaem_1GQrA1IwNqj0GGsarBm5sb5CmKQuK3jiDnaz2fWyDVNKCFFcWjSleKPVJIr3FPZImOrFdiM-GmNpcZnhdDkM21GbemC2Fi1Wr0-v0j5SJi0mzr3QI2ZYrZmQVoucBoccrVc4Il3KRtpwxruGld-pNN-Ot3OuQLRlACUVMk8QtRDofurUz5M4lnOoGgfBh3GUpk_nR2SU4gSNbtRm36_C6rFt47URVOQWhmogYN5RPIaANfD4O4QJ9inTqO.hk-krhoHm_Ek5A8n-xmsLUpvKhU4luQo80vj6-WMTf4");
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
