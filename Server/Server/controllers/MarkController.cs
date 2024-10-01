using WebServer.Server.config;
using WebServer.Server.repositorys;
using static WebServer.Server.config.Database;
using System.Data.Common;

namespace WebServer.Server.controllers
{
    internal class MarkController : BaseController
    {
        private static DbCommand CreateCommandWithParameters(DbConnection connection, string query, params (string name, object value)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;

            foreach (var (name, value) in parameters)
            {
                AddParameter(command, name, value);
            }

            return command;
        }

        public static List<MarkRepository> GetMarksForLessons(string studentId, string lessonId)
        {
            var marks = new List<MarkRepository>();
            var db = new Database(DatabaseType.MySQL);

            try
            {
                db.Connect_to_Database();
                var connection = db.GetConnection();
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

                using var command = CreateCommandWithParameters((DbConnection)connection, query,
                    ("@studentId", studentId),
                    ("@lessonId", lessonId));

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
            finally
            {
                db.Close_Connection();
            }

            return marks;
        }

        public static string UpdateMarkAsTeacher(string studentId, string teacherId, string lessonId, string teacherMark, string finalMark)
        {
            string message = "Update successful";
            var db = new Database(DatabaseType.MySQL);

            try
            {
                db.Connect_to_Database();
                var connection = db.GetConnection();
                string query = @"
                    UPDATE marks 
                    SET teacher_mark = @teacherMark,
                        final_mark = @finalMark,
                        teacher_id = @teacherId
                    WHERE student_id = @studentId
                    AND lesson_id = @lessonId";

                using var command = CreateCommandWithParameters((DbConnection)connection, query,
                    ("@studentId", studentId),
                    ("@teacherId", teacherId),
                    ("@lessonId", lessonId),
                    ("@teacherMark", teacherMark),
                    ("@finalMark", finalMark));

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
            finally
            {
                db.Close_Connection();
            }

            return message;
        }

        public static string UpdateMarkAsStudent(string studentId, string lessonId, string studentMark)
        {
            string message = "Update successful";
            var db = new Database(DatabaseType.MySQL);

            try
            {
                db.Connect_to_Database();
                var connection = db.GetConnection();
                string query = @"
                    UPDATE marks 
                    SET student_mark = @studentMark  
                    WHERE student_id = @studentId
                    AND lesson_id = @lessonId";

                using var command = CreateCommandWithParameters((DbConnection)connection, query,
                    ("@studentId", studentId),
                    ("@lessonId", lessonId),
                    ("@studentMark", studentMark));

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
            finally
            {
                db.Close_Connection();
            }

            return message;
        }
    }
}
