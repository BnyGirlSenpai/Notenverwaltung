using WebServer.Server.repositorys;

namespace WebServer.Server.controllers
{
    internal class MarkController : BaseController, IDisposable
    {
        public MarkController() 
        {
           ConnectToDatabase();
        }

        public List<MarkRepository> GetMarksForLessons(string studentId, string lessonId)
        {
            var marks = new List<MarkRepository>();

            try
            {
               
                string query = @"
                    SELECT m.mark_id, 
                           m.student_mark, 
                           m.teacher_mark, 
                           m.final_mark, 
                           u.last_name, 
                           u.first_name
                    FROM marks m
                    LEFT JOIN users u ON m.teacher_id = u.user_id
                    WHERE m.student_id = @studentId AND m.lesson_id = @lessonId;";

                using var command = CreateCommand(query);
                AddParameter(command, "@studentId", studentId);
                AddParameter(command, "@lessonId", lessonId);

                marks = ExecuteReader(command, reader => new MarkRepository
                {
                    MarkId = reader["mark_id"]?.ToString() ?? "Unknown",
                    StudentMark = reader["student_mark"]?.ToString() ?? "Unknown",
                    TeacherMark = reader["teacher_mark"]?.ToString() ?? "Unknown",
                    FinalMark = reader["final_mark"]?.ToString() ?? "Unknown",
                    TeacherFirstname = reader["first_name"]?.ToString() ?? "Unknown",
                    TeacherLastname = reader["last_name"]?.ToString() ?? "Unknown",
                });

                if (marks.Count == 0)
                {
                    marks.Add(new MarkRepository
                    {
                        MarkId = "N.a.N",
                        StudentMark = "N.a.N",
                        TeacherMark = "N.a.N",
                        FinalMark = "N.a.N",
                        TeacherFirstname = "N.a.N",
                        TeacherLastname = "N.a.N",
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching marks: {ex.Message}");
            }
      
            return marks;
        }

        public string UpdateMarkAsTeacher(string studentId, string teacherId, string lessonId, string teacherMark, string finalMark)
        {
            string message = "Update successful";

            try
            {
                string query = @"
                    UPDATE marks 
                    SET teacher_mark = @teacherMark,
                        final_mark = @finalMark,
                        teacher_id = @teacherId
                    WHERE student_id = @studentId
                    AND lesson_id = @lessonId";

                using var command = CreateCommand(query);
                AddParameter(command, "@studentId", studentId);
                AddParameter(command, "@teacherId", teacherId);
                AddParameter(command, "@lessonId", lessonId);
                AddParameter(command, "@teacherMark", teacherMark);
                AddParameter(command, "@finalMark", finalMark);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    message = "No record found to update";
                }
            }
            catch (Exception ex)
            {
                message = $"Error updating marks: {ex.Message}";
            }

            return message;
        }

        public string UpdateMarkAsStudent(string studentId, string lessonId, string studentMark)
        {
            string message = "Update successful";

            try
            {            
                string query = @"
                    UPDATE marks 
                    SET student_mark = @studentMark  
                    WHERE student_id = @studentId
                    AND lesson_id = @lessonId";

                using var command = CreateCommand(query);
                AddParameter(command, "@studentId", studentId);
                AddParameter(command, "@lessonId", lessonId);
                AddParameter(command, "@studentMark", studentMark);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    message = "No record found to update";
                }
            }
            catch (Exception ex)
            {
                message = $"Error updating marks: {ex.Message}";
            }

            return message;
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
