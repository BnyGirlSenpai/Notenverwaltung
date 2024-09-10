namespace App.App.services
{
    internal class LoginService
    {
        public static async Task<bool> LoginAsync()
        {
            bool authenticated = await Authenticator.AuthenticateAsync();

            return authenticated;
        }

        public static async Task LogoutAsync()
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
}
