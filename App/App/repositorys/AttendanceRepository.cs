using System.Text.Json.Serialization;

namespace App.App.repositorys
{
    internal class AttendanceRepository 
    {
        [JsonPropertyName("attendanceId")]
        public string AttendanceId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
