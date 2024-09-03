using NotenverwaltungsApp.App.utils;
using System;
using System.IO;

namespace NotenverwaltungsApp.App.services
{
    internal class TokenService
    {
        private static readonly string TokenFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token.dat");

        public static void SaveToken(string tokenJson)
        {
            try
            {
                var tokenGenerator = new TokenGenerator();
                string jwtToken = tokenGenerator.GenerateJwtToken(tokenJson);
                File.WriteAllText(TokenFilePath, jwtToken);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Fehler beim Speichern des Tokens: {ex.Message}");
            }
        }

        public static string LoadToken()
        {
            try
            {
                if (File.Exists(TokenFilePath))
                {
                    return File.ReadAllText(TokenFilePath);
                }
                return null;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Fehler beim Laden des Tokens: {ex.Message}");
                return null;
            }
        }

        public static void DeleteToken()
        {
            try
            {
                if (File.Exists(TokenFilePath))
                {
                    File.Delete(TokenFilePath);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Fehler beim Löschen des Tokens: {ex.Message}");
            }
        }
    }
}
