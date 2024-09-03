namespace App.App.services
{
    internal class AuthenticationService
    {
        private static readonly HttpClient _client = new();

        public static async Task<string> AuthenticateUserCredentials(string username, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/auth/login")
            {
                Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            ])
            };

            try
            {
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }
    }
}
