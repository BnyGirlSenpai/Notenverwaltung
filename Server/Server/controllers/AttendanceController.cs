using HttpServer.Server.repositorys;

namespace HttpServer.Server.controllers
{
    internal class AttendanceController : BaseController, IDisposable
    {
        public AttendanceController() 
        {
            ConnectToDatabase();
        }   

        public List<AttendanceRepository> GetAttendanceForLesson(string studentId, string lessonId)
        {
            var attendances = new List<AttendanceRepository>();

            try
            {
                string query = @"
                    SELECT attendance_id, status 
                    FROM attendance 
                    WHERE student_id = @studentId 
                    AND lesson_id = @lessonId
                ";

                using var command = CreateCommand(query);          
                AddParameter(command, "@studentId", studentId);
                AddParameter(command, "@lessonId", lessonId);

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
        
            return attendances;
        }

        public string UpdateAttendanceForLesson(string studentId, string lessonId, string attendanceStatus)
        {
            string message = "Update successful";

            try
            {            
                string query = @"
                    UPDATE attendance 
                    SET status = @status 
                    WHERE student_id = @studentId 
                    AND lesson_id = @lessonId
                ";

                using var command = CreateCommand(query);
                AddParameter(command, "@studentId", studentId);
                AddParameter(command, "@lessonId", lessonId);
                AddParameter(command, "@status", attendanceStatus);

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

            return message;
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
