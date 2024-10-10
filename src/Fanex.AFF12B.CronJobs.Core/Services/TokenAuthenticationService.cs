using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserManagementService _userManagementService;
        private readonly TokenManagementOptions _tokenManagement;
        public TokenAuthenticationService(IUserManagementService service,
            IOptions<TokenManagementOptions> tokenManagement)
        {
            _userManagementService = service;
            _tokenManagement = tokenManagement.Value;
        }

        public Tuple<bool, Token> IsAuthenticated(TokenRequestParam param)
        {
            var token = new Token
            {
                ExpiresIn = _tokenManagement.AccessExpiration
            };
            var siteName = param.Username;
            var realUserName = param.Username;

            if (param.Username.Contains('@'))
            {
                var temp = param.Username.Split('@');
                siteName = temp[0];
                realUserName = temp[1];
            }

            if (!_userManagementService.IsValidUser(realUserName, param.Password))
            {
                return Tuple.Create<bool, Token>(false, token);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, realUserName),
                new Claim(CustomClaimType.Site, siteName),
                new Claim(CustomClaimType.IsShowIPVN, param.IsShowIpvn.ToString(), ClaimValueTypes.Boolean)
             };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var encryptCredentials = new EncryptingCredentials(key, JwtConstants.DirectKeyUseAlg,
                SecurityAlgorithms.Aes256CbcHmacSha512);

            var utcDateTimeNow = DateTimeOffset.Now.UtcDateTime;
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.CreateJwtSecurityToken(_tokenManagement.Issuer,
                  _tokenManagement.Audience,
                  new ClaimsIdentity(claims),
                  utcDateTimeNow,
                  utcDateTimeNow.AddMinutes(_tokenManagement.AccessExpiration),
                  utcDateTimeNow,
                  credentials,
                  encryptCredentials
                  );

            token.AccessToken = handler.WriteToken(jwtToken);
            token.TokenType = handler.TokenType.Name;
            return Tuple.Create(true, token);
        }
    }
}
