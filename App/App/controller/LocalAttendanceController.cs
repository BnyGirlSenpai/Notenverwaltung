using App.App.repositorys;
using System.Data.SQLite;

namespace App.App.controller
{
    internal class LocalAttendanceController : LocalBaseController
    {
        public static async Task<List<AttendanceRepository>> GetAttendanceForLessonAsync(string studentId, string lessonId)
        {
            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand("SELECT attendance_id, status FROM attendance WHERE student_id = @studentId AND lesson_id = @lessonId", connection);
            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@lessonId", lessonId);

            var attendances = await ExecuteQueryAsync(command, async reader => new AttendanceRepository
            {
                AttendanceId = reader["attendance_id"]?.ToString() ?? "Unknown",
                Status = reader["status"]?.ToString() ?? "Unknown"
            });

            if (attendances.Count == 0)
            {
                attendances.Add(new AttendanceRepository
                {
                    AttendanceId = "N.a.N",
                    Status = "N.a.N"
                });
            }

            return attendances;
        }

        public static async Task<string> UpdateAttendanceForLessonAsync(string studentId, string lessonId, string newAttendanceStatus)
        {
            string message = "Update successful";

            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand("UPDATE attendance SET status = @status WHERE student_id = @studentId AND lesson_id = @lessonId", connection);
            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@lessonId", lessonId);
            command.Parameters.AddWithValue("@status", newAttendanceStatus);

            try
            {
                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    message = "No record found to update";
                }

                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating attendance: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}
