using System.Text.Json.Serialization;

namespace HttpServer.Server.repositorys
{
    internal class AttendanceRepository
    {
        [JsonPropertyName("attendanceId")]
        public string AttendanceId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
