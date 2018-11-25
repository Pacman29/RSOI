using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;

namespace AuthOptions
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly AuthOptions _options;

        public JwtTokenGenerator(AuthOptions options)
        {
            _options = options;
        }
        
        public JwtSecurityToken GenerateJwtToken(string id, string userName)
        {
            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub, id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                new Claim(JwtRegisteredClaimNames.Iss, _options.ISSUER),
                new Claim(JwtRegisteredClaimNames.Iss, _options.AUDIENCE),
            };

            var key = _options.GetSymmetricSecurityKey();
            var credits = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_options.ISSUER,
                _options.AUDIENCE,
                claims,
                expires: DateTime.Now.Add(TimeSpan.FromMinutes(_options.Lifetime)),
                signingCredentials: credits);

            return token;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = _options.GetParameters();
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}