using NotenverwaltungsApp.App.services;
using NotenverwaltungsApp.App.utils;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

internal class Program
{
    static async Task Main()
    {
        bool isAuthenticated = await LoginAsync();

        if (isAuthenticated)
        {
            while (true)
            {
                await GetUserInfo();
                Console.WriteLine("Geben Sie 'logout' ein, um sich abzumelden.");
                var input = Console.ReadLine();
                if (input?.ToLower() == "logout")
                {
                    await LogoutAsync();
                    Console.WriteLine("Sie wurden abgemeldet. Programm wird beendet.");
                    break;
                }
                else
                {
                    Console.WriteLine("Ungültiger Befehl. Geben Sie 'logout' ein, um sich abzumelden.");
                }
            }
        }
        else
        {
            Console.WriteLine("Programm wird beendet.");
        }
    }

    private static async Task<bool> LoginAsync()
    {
        var authenticator = new Authenticator();
        bool authenticated = await authenticator.AuthenticateAsync();

        return authenticated;
    }

    private static async Task GetUserInfo()
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

                Console.WriteLine($"Name: {firstNameElement.GetString()} {lastNameElement.GetString()}");
                Console.WriteLine($"Logged in as {roleElement.GetString()}");
            }
            else
            {
                Console.WriteLine("Failed to parse user information.");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing user information: {ex.Message}");
        }
    }

    private static async Task LogoutAsync()
    {
        try
        {
            TokenService.DeleteToken();
            Console.WriteLine("Abmeldung erfolgreich.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler bei der Abmeldung: {ex.Message}");
        }
    }
}
