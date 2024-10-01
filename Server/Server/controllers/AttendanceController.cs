using System.Data.Common;
using WebServer.Server.config;
using WebServer.Server.repositorys;
using static WebServer.Server.config.Database;

namespace WebServer.Server.controllers
{
    internal class AttendanceController : BaseController
    {
        public static List<AttendanceRepository> GetAttendanceForLesson(string studentId, string lessonId)
        {
            var attendances = new List<AttendanceRepository>();

            using var db = new Database(DatabaseType.MySQL);
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

                using var command = CreateCommandWithParameters((DbConnection)connection, query,
                [
                    ("@studentId", studentId),
                    ("@lessonId", lessonId)
                ]);

                using var reader = command.ExecuteReader();
                while (reader.Read())
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching attendance: {ex.Message}");
            }
            finally
            {
                db.Close_Connection();
            }

            return attendances;
        }

        public static string UpdateAttendanceForLesson(string studentId, string lessonId, string attendanceStatus)
        {
            string message = "Update successful";

            using var db = new Database(DatabaseType.MySQL);
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

                using var command = CreateCommandWithParameters((DbConnection)connection, query,
                [
                    ("@studentId", studentId),
                    ("@lessonId", lessonId),
                    ("@status", attendanceStatus)
                ]);

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

            return message;
        }

        private static DbCommand CreateCommandWithParameters(DbConnection connection, string query, (string ParameterName, object Value)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;

            foreach (var parameter in parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.ParameterName;
                dbParameter.Value = parameter.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }
    }
}
