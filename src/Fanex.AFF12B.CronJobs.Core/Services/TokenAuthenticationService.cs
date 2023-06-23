using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
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
            IOptions<TokenManagementOptions> tokenManagementAccessor)
        {
            _userManagementService = service;
            _tokenManagement = tokenManagementAccessor.Value;
        }

        public Tuple<bool, Token> IsAuthenticated(TokenRequestParam requestParam)
        {
            var token = new Token
            {
                ExpiresIn = _tokenManagement.AccessExpiration
            };
            var siteName = requestParam.Username;
            var realUserName = requestParam.Username;

            if (requestParam.Username.Contains('@'))
            {
                var temp = requestParam.Username.Split('@');
                siteName = temp[0];
                realUserName = temp[1];
            }

            if (!_userManagementService.IsValidUser(realUserName, requestParam.Password))
            {
                return Tuple.Create<bool, Token>(false, token);
            }

            var claim = new[]
            {
                new Claim(ClaimTypes.Name, siteName),
                new Claim(CustomClaimType.IsShowIPVN, requestParam.IsShowIpvn.ToString(), ClaimValueTypes.Boolean)
             };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var encryptCredenction = new EncryptingCredentials(key, JwtConstants.DirectKeyUseAlg,
                SecurityAlgorithms.Aes256CbcHmacSha512);
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.CreateJwtSecurityToken(_tokenManagement.Issuer,
                  _tokenManagement.Audience,
                  new ClaimsIdentity(claim),
                  DateTime.Now,
                  DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration),
                  null,
                  credentials,
                  encryptCredenction
                  );
            token.AccessToken = handler.WriteToken(jwtToken);
            token.TokenType = handler.TokenType.Name;
            return Tuple.Create<bool, Token>(true, token);
        }
    }
}
