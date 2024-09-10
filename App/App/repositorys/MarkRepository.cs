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

        [JsonPropertyName("endMark")]
        public string FinalMark { get; set; }

        [JsonPropertyName("teacherName")]
        public string TeacherName { get; set; }
    }
}
