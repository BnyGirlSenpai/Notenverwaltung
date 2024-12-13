using System.Text.Json;

namespace App.App.api
{
    internal class BaseApi
    {
        protected static async Task<string> SendGetRequest(string url, Dictionary<string, string> content)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Content = new FormUrlEncodedContent(content)
            };

            try
            {
                using HttpClient _client = new();
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null; 
            }
        }

        // Für weitere Funktionen wie Notenerstellung usw.
        protected static async Task<string> SendPostRequest(string url, Dictionary<string, string> content)
        {
            using HttpClient client = new();
            var response = await client.PostAsync(url, new FormUrlEncodedContent(content));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected static async Task<string> SendPutRequest(string url, Dictionary<string, string> content)
        {
            using HttpClient client = new();
            var response = await client.PutAsync(url, new FormUrlEncodedContent(content));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected static bool IsOffline(string connectionStatus)
        {
            return connectionStatus.Equals("Offline", StringComparison.OrdinalIgnoreCase);
        }

        protected static async Task<List<T>> DeserializeJsonAsync<T>(string jsonData)
        {
            return JsonSerializer.Deserialize<List<T>>(jsonData);
        }
    }
}
