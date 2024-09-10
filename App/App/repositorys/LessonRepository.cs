using System.Text.Json.Serialization;

namespace App.App.repositorys
{
    internal class LessonRepository
    {
        [JsonPropertyName("lessonName")]
        public string LessonName { get; set; }

        [JsonPropertyName("lessonDate")]
        public string LessonDate { get; set; }

        [JsonPropertyName("lessonId")]
        public string LessonId { get; set; }
    }
}
