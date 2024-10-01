using System.Text.Json.Serialization;

namespace WebServer.Server.repositorys
{
    internal class LessonRepository
    {
        [JsonPropertyName("lessonId")]
        public string LessonId { get; set; }

        [JsonPropertyName("lessonName")]
        public string LessonName { get; set; }

        [JsonPropertyName("lessonDate")]
        public string LessonDate { get; set; }

        [JsonPropertyName("lessonType")]
        public string LessonType { get; set; }
    }
}
