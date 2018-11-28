using System.IdentityModel.Tokens.Jwt;

namespace AuthOptions
{
    public interface IJwtTokenGenerator
    {
        JwtSecurityToken GenerateJwtToken(string id, string userName);
        bool ValidateToken(string token);
    }
}