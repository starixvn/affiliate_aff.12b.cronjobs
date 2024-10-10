using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Fanex.AFF12B.CronJobs.Core.Extensions
{
    public static class HttpContextExtensions
    {
        private const int CookieExpirationMinutes = 30;
        private const string Aff12BHangFireCookieName = "Aff_12B_HangFire_Cookie";

        public static bool IsAuthorize(this HttpContext httpContext, TokenValidationParameters tokenValidationParameters)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return true;
            }

            var shouldSetCookie = false;

            string accessToken;
            if (httpContext.Request.Query.ContainsKey("access_token"))
            {
                shouldSetCookie = true;
                accessToken = httpContext.Request.Query["access_token"].FirstOrDefault();
            }
            else
            {
                accessToken = httpContext.Request.Cookies[Aff12BHangFireCookieName];
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var claims = handler.ValidateToken(accessToken, tokenValidationParameters, out _);
                    if (claims is null || !claims.Identity.IsAuthenticated)
                    {
                        return false;
                    }

                    if (shouldSetCookie)
                    {
                        httpContext.Response.Cookies.Append(Aff12BHangFireCookieName,
                            accessToken,
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.Now.AddMinutes(CookieExpirationMinutes),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Log.Warn(ErrorMessageHelper.GenerateMessage(ex));
                }
            }

            return false;
        }
    }
}
