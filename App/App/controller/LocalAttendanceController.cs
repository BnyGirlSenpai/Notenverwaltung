using App.App.repositorys;
using System.Data.SQLite;

namespace App.App.controller
{
    internal class LocalAttendanceController
    {
        public static async Task<List<AttendanceRepository>> GetAttendanceForLessonAsync(string studentId, string lessonId)
        {
            try
            {
                using var connection = new SQLiteConnection("Data Source=C:\\Users\\drebes\\Berufsschule\\SDM\\SQL\\Database\\Notenverwaltung.db3;Version=3;");
                await connection.OpenAsync();

                var command = new SQLiteCommand(" SELECT attendance_id, status FROM attendance WHERE student_id = @studentId AND lesson_id = @lessonId", connection);
                command.Parameters.AddWithValue("@studentId", studentId);
                command.Parameters.AddWithValue("@lessonId", lessonId);

                var attendances = new List<AttendanceRepository>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var attendance = new AttendanceRepository
                        {
                            AttendanceId = reader["attendance_id"]?.ToString() ?? "Unknown",
                            Status = reader["status"]?.ToString() ?? "Unknown",
                        };
                        attendances.Add(attendance);
                    }

                    if (attendances.Count == 0)
                    {
                        attendances.Add(new AttendanceRepository
                        {
                            AttendanceId = "N.a.N",
                            Status = "N.a.N",
                        });
                    }
                }

                return attendances;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching courses: {ex.Message}");
                return [];
            }
        }
    }
}

