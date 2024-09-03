using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace App.App.utils
{
    internal class TokenExtractor
    {
        private readonly string _secretKey;

        public TokenExtractor(string secretKey)
        {
            _secretKey = secretKey;
        }

        public string ExtractUserInfoFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey))
                };

                var principal = handler.ValidateToken(token, validationParameters, out var securityToken);
                var jwtToken = securityToken as JwtSecurityToken;

                if (jwtToken == null)
                {
                    Console.WriteLine("Invalid token.");
                    return "Invalid token.";
                }

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var firstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
                var lastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
                var userRole = jwtToken.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                var userInfo = new
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    Role = userRole
                };

                return JsonSerializer.Serialize(userInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting token information: {ex.Message}");
                return $"Error extracting token information: {ex.Message}";
            }
        }
    }
}
