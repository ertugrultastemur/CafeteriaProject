using Castle.Components.DictionaryAdapter.Xml;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Encryption;
using Core.Utilities.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.JWT
{
    public class JwtHelper : ITokenHelper
    {

        public IConfiguration Configuration { get; }

        private TokenOptions _tokenOptions;

        private DateTime _accessTokenExpiration;
        private DateTime _refreshTokenExpiration;

        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
        }

        public AccessToken CreateAccessToken(User user, List<OperationClaim> operationClaims)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            JwtSecurityToken jwt = CreateAccessJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims);
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            string token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public AccessToken CreateRefreshToken(User user)
        {
            _refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.RefreshTokenExpiration);
            SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);


            JwtSecurityToken jwt = CreateRefreshJwtSecurityToken(_tokenOptions, user, signingCredentials);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            string refreshToken = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken
            {
                Token = refreshToken,
                Expiration = _refreshTokenExpiration
            };
        }

        private JwtSecurityToken CreateAccessJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials, List<OperationClaim> operationClaims)
        {
            try
            {
                JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, operationClaims),
                signingCredentials: signingCredentials
                );
                return jwt;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        private JwtSecurityToken CreateRefreshJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials)
        {
            try
            {
                JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user),
                signingCredentials: signingCredentials
                );
                return jwt;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        private IEnumerable<Claim> SetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            return claims;
        }

        private IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
        {
            var _claims = new List<Claim>();
            _claims.AddNameIdentifier(user.Id.ToString());
            _claims.AddName($"{user.FirstName} {user.LastName}");
            _claims.AddEmail(user.Email);
            _claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());
            return _claims;
        }

        public IDataResult<int?> GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return new ErrorDataResult<int?>("Token okunamadı.");

            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId");
            if (userIdClaim == null)
                return new ErrorDataResult<int?>("Token içinde id claimi bulunamadı.");

            return new SuccessDataResult<int?>(int.TryParse(userIdClaim.Value, out int userId) ? userId : null);
        }
    }
}
