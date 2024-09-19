using System.Text.Json.Serialization;

namespace App.App.repositorys
{
    internal class MarkRepository
    {
        [JsonPropertyName("markId")]
        public string MarkId { get; set; }

        [JsonPropertyName("studentMark")]
        public string StudentMark { get; set; }

        [JsonPropertyName("teacherMark")]
        public string TeacherMark { get; set; }

        [JsonPropertyName("finalMark")]
        public string FinalMark { get; set; }

        [JsonPropertyName("teacherId")]
        public string TeacherId { get; set; }
    }
}
