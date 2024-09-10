using NotenverwaltungsApp;
using System.Text.Json.Serialization;
using static NotenverwaltungsApp.Database;

namespace Server.Server.controllers
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

            [JsonPropertyName("endMark")]
            public string EndMark { get; set; }

            [JsonPropertyName("teacherName")]
            public string TeacherName { get; set; }
        }

        public static List<Mark> GetMarksForLessons(string userId, string lessonId)
        {
            var marks = new List<Mark>();

            using var db = new Database(DatabaseType.SQLite);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                       ";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var userparameter = command.CreateParameter();
                    userparameter.ParameterName = "@userId";
                    userparameter.Value = userId;
                    command.Parameters.Add(userparameter);

                    var lessonsparameter = command.CreateParameter();
                    lessonsparameter.ParameterName = "@lessonId";
                    lessonsparameter.Value = lessonId;
                    command.Parameters.Add(lessonsparameter);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var mark = new Mark
                        {
                            MarkId = reader["´mark_id"]?.ToString() ?? "Unknown",
                            StudentMark = reader["student_mark"]?.ToString() ?? "Unknown",
                            TeacherMark = reader["teacher_mark"]?.ToString() ?? "Unknown",
                            EndMark = reader["student_mark"]?.ToString() ?? "Unknown",
                            TeacherName = reader["teacher_name"]?.ToString() ?? "Unknown",
                        };
                        marks.Add(mark);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching courses: {ex.Message}");
                }
                finally
                {
                    db.Close_Connection();
                }
            }

            return marks;
        }
    }
}
