using App.App.services;

internal class Authenticator
{
    public static async Task<bool> AuthenticateAsync()
    {
        string token = TokenService.LoadToken();
        bool authenticated = false;

        while (!authenticated)
        {
            if (token == null)
            {
                Console.WriteLine("Bitte geben Sie Ihren Benutzernamen ein:");
                var username = Console.ReadLine();
                Console.WriteLine("Bitte geben Sie Ihr Passwort ein:");
                var password = Console.ReadLine();

                try
                {
                    token = await AuthenticationService.AuthenticateUserCredentials(username, password);
                    if (token != null)
                    {
                        TokenService.SaveToken(token);
                        authenticated = true;
                        Console.WriteLine("Authentifizierung erfolgreich.");
                    }
                    else
                    {
                        Console.WriteLine("Authentifizierung fehlgeschlagen.");
                        if (!RetryAuthentication())
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler bei der Authentifizierung: {ex.Message}");
                    if (!RetryAuthentication())
                    {
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("Authentifizierung erfolgreich.");
                authenticated = true;
            }
        }
        return true;
    }

    private static bool RetryAuthentication()
    {
        Console.WriteLine("Möchten Sie es erneut versuchen? (y/n)");
        var retry = Console.ReadLine();
        return retry?.ToLower() == "y";
    }
}
