using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using App.App.repositorys;

namespace App.App.utils
{
    internal class TokenGenerator
    {
        public static string GenerateJwtToken(string tokenJson)
        {
            var tokenData = JsonSerializer.Deserialize<TokenRepository>(tokenJson);

            if (tokenData != null)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("u3rZ8BaR5WzCnP7GdT3JPEFbL0hG5lWm5F0q9PT0Ri8=\r\n"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                new Claim("FirstName", tokenData.FirstName),
                new Claim("LastName", tokenData.LastName),
                new Claim("UserId", tokenData.UserId),
                new Claim("Role", tokenData.Role),
                new Claim("Message", tokenData.Message)
            };

                var token = new JwtSecurityToken(
                    issuer: "yourdomain.com",
                    audience: "yourdomain.com",
                    claims: claims,
                    expires: DateTime.Now.AddYears(1),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            throw new ArgumentException("Invalid token data");
        }
    }
}
