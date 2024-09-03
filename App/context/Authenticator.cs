using NotenverwaltungsApp.App.services;
using System;
using System.Threading.Tasks;
<<<<<<< HEAD:App/context/Authenticator.cs
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
=======
>>>>>>> 9cc59d4096c30fb1d4c196abfbd5453342357e52:App/App/context/Authenticator.cs

internal class Authenticator
{
    public Authenticator()
    {
        
    }

    public async Task<bool> AuthenticateAsync()
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

    private bool RetryAuthentication()
    {
        Console.WriteLine("Möchten Sie es erneut versuchen? (y/n)");
        var retry = Console.ReadLine();
        return retry?.ToLower() == "y";
    }
}
