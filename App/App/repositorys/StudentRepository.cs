using System.Text.Json.Serialization;

namespace App.App.repositorys
{
    internal class StudentRepository
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
