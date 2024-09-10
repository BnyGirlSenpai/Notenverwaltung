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

        [JsonPropertyName("teacherFirstname")]
        public string TeacherFirstname { get; set; }

        [JsonPropertyName("teacherLastname")]
        public string TeacherLastname { get; set; }
    }
}
