using System.Text.Json.Serialization;

namespace HttpServer.Server.repositorys
{
    internal class StudentRepository
    {
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
