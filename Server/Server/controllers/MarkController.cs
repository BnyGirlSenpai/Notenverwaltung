using System.Text.Json.Serialization;
using static NotenverwaltungsApp.Database;

namespace NotenverwaltungsApp.Server.controllers
{
    internal class MarkController
    {
        public class Mark
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

        public static List<Mark> GetMarksForLessons(string studentId, string lessonId)
        {
            var marks = new List<Mark>();

            using var db = new Database(DatabaseType.MySQL);
            {
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
                        var mark = new Mark
                        {
                            MarkId = reader["mark_id"]?.ToString() ?? "Unknown",
                            StudentMark = reader["student_mark"]?.ToString() ?? "Unknown",
                            TeacherMark = reader["teacher_mark"]?.ToString() ?? "Unknown",
                            FinalMark = reader["final_mark"]?.ToString() ?? "Unknown",
                            TeacherFirstname = reader["first_name"]?.ToString() ?? "Unknown",
                            TeacherLastname = reader["last_name"]?.ToString() ?? "Unknown",
                        };
                        marks.Add(mark);
                    }

                    if (marks.Count == 0)
                    {
                        marks.Add(new Mark
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
            }

            return marks;
        }

        public static string UpdateMarkAsTeacher(string studentId, string teacherId, string lessonId, string teacherMark, string finalMark)
        {
            string message = "Update successful";

            using var db = new Database(DatabaseType.MySQL);
            {
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
                        AND lesson_id = @lessonId
                    ";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var studentParameter = command.CreateParameter();
                    studentParameter.ParameterName = "@studentId";
                    studentParameter.Value = studentId;
                    command.Parameters.Add(studentParameter);

                    var teacherParameter = command.CreateParameter();
                    teacherParameter.ParameterName = "@teacherId";
                    teacherParameter.Value = teacherId;
                    command.Parameters.Add(teacherParameter);

                    var lessonParameter = command.CreateParameter();
                    lessonParameter.ParameterName = "@lessonId";
                    lessonParameter.Value = lessonId;
                    command.Parameters.Add(lessonParameter);

                    var teacherMarkParameter = command.CreateParameter();
                    teacherMarkParameter.ParameterName = "@teacherMark";
                    teacherMarkParameter.Value = teacherMark;
                    command.Parameters.Add(teacherMarkParameter);

                    var finalMarkParameter = command.CreateParameter();
                    finalMarkParameter.ParameterName = "@finalMark";
                    finalMarkParameter.Value = finalMark;
                    command.Parameters.Add(finalMarkParameter);

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
            }

            return message;
        }

        public static string UpdateMarkAsStudent(string studentId, string lessonId, string studentMark)
        {
            string message = "Update successful";

            using var db = new Database(DatabaseType.MySQL);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        UPDATE marks 
                        SET student_mark = @studentMark  
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

                    var studentMarkParameter = command.CreateParameter();
                    studentMarkParameter.ParameterName = "@studentMark";
                    studentMarkParameter.Value = studentMark;
                    command.Parameters.Add(studentMarkParameter);

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
            }

            return message;
        }

    }
}
