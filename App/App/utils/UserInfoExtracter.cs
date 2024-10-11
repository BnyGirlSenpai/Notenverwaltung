using App.App.services;
using System.Text.Json;

namespace App.App.utils
{
    internal class UserInfoExtractor
    {
        public static async Task<(string role, string firstName, string lastName, string userId)> GetUserInfo()
        {
            string token = TokenService.LoadToken();
            string secretKey = "u3rZ8BaR5WzCnP7GdT3JPEFbL0hG5lWm5F0q9PT0Ri8=\r\n";
            var tokenExtractor = new TokenExtractor(secretKey);
            string userInfoJson = tokenExtractor.ExtractUserInfoFromToken(token);

            try
            {
                var userInfo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userInfoJson);

                if (userInfo != null)
                {
                    userInfo.TryGetValue("FirstName", out JsonElement firstNameElement);
                    userInfo.TryGetValue("LastName", out JsonElement lastNameElement);
                    userInfo.TryGetValue("Role", out JsonElement roleElement);
                    userInfo.TryGetValue("UserId", out JsonElement userIdElement);

                    string userId = userIdElement.GetString() ?? "Unknown";
                    string firstName = firstNameElement.GetString() ?? "Unknown";
                    string lastName = lastNameElement.GetString() ?? "Unknown";
                    string role = roleElement.GetString() ?? "Unknown";

                    Console.WriteLine($"User information:{userId}");
                    Console.WriteLine($"Name: {firstName} {lastName}");
                    Console.WriteLine($"Logged in as {role}");

                    return (role, firstName, lastName, userId);
                }
                else
                {
                    Console.WriteLine("Failed to parse user information.");
                    return (string.Empty, string.Empty, string.Empty, string.Empty);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing user information: {ex.Message}");
                return (string.Empty, string.Empty, string.Empty, string.Empty);
            }
        }
    }
}
