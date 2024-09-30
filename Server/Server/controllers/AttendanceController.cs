using System.Text.Json.Serialization;
using WebServer.Server.config;
using static WebServer.Server.config.Database;

namespace WebServer.Server.controllers
{
    internal class AttendanceController
    {
        public class Attendance
        {
            [JsonPropertyName("attendanceId")]
            public string AttendanceId { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }
        }

        public static List<Attendance> GetAttendanceForLesson(string studentId, string lessonId)
        {
            var attendances = new List<Attendance>();

            using var db = new Database(DatabaseType.MySQL);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        SELECT attendance_id, status 
                        FROM attendance 
                        WHERE student_id = @studentId 
                        AND lesson_id = @lessonId
                    ";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var studentParameter = command.CreateParameter();
                    studentParameter.ParameterName = "@studentId";
                    studentParameter.Value = studentId;
                    command.Parameters.Add(studentParameter);

                    var lessonParameter = command.CreateParameter();
                    lessonParameter.ParameterName = "@lessonId";
                    lessonParameter.Value = lessonId;
                    command.Parameters.Add(lessonParameter);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var attendance = new Attendance
                        {
                            AttendanceId = reader["attendance_id"]?.ToString() ?? "Unknown",
                            Status = reader["status"]?.ToString() ?? "Unknown",

                        };
                        attendances.Add(attendance);
                    }

                    if (attendances.Count == 0)
                    {
                        attendances.Add(new Attendance
                        {
                            AttendanceId = "N.a.N",
                            Status = "N.a.N",
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching marks: {ex.Message}");
                }
                finally
                {
                    db.Close_Connection();
                }
            }
            return attendances;
        }

        public static string UpdateAttendanceForLesson(string studentId, string lessonId, string attendanceStatus)
        {
            string message = "Update successful";

            using var db = new Database(DatabaseType.MySQL);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        UPDATE attendance 
                        SET status = @status 
                        WHERE student_id = @studentId 
                        AND lesson_id = @lessonId
                    ";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var studentParameter = command.CreateParameter();
                    studentParameter.ParameterName = "@studentId";
                    studentParameter.Value = studentId;
                    command.Parameters.Add(studentParameter);

                    var lessonParameter = command.CreateParameter();
                    lessonParameter.ParameterName = "@lessonId";
                    lessonParameter.Value = lessonId;
                    command.Parameters.Add(lessonParameter);

                    var statusParameter = command.CreateParameter();
                    statusParameter.ParameterName = "@status";
                    statusParameter.Value = attendanceStatus;
                    command.Parameters.Add(statusParameter);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        message = "No record found to update";
                    }
                }
                catch (Exception ex)
                {
                    message = $"Error updating attendance: {ex.Message}";
                }
                finally
                {
                    db.Close_Connection();
                }
            }

            return message;
        }
    }
}


