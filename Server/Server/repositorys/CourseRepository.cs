using System.Text.Json.Serialization;

namespace HttpServer.Server.repositorys
{
    internal class CourseRepository
    {
        [JsonPropertyName("courseCode")]
        public string CourseCode { get; set; }

        [JsonPropertyName("courseName")]
        public string CourseName { get; set; }

        [JsonPropertyName("courseId")]
        public string CourseId { get; set; }
    }
}
