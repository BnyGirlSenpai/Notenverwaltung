using System.Text.Json.Serialization;
using static NotenverwaltungsApp.Database;

namespace NotenverwaltungsApp.Server.controllers
{
    internal class CourseController
    {
        public class Course
        {
            [JsonPropertyName("courseCode")]
            public string CourseCode { get; set; }
            [JsonPropertyName("courseName")]
            public string CourseName { get; set; }
        }

        public static List<Course> GetCoursesByTeacher(string teacherId)
        {
            var courses = new List<Course>();

            using var db = new Database(DatabaseType.SQLite);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        SELECT course_code, course_name
                        FROM courses
                        WHERE teacher_id = @TeacherId";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var teacherIdParameter = command.CreateParameter();
                    teacherIdParameter.ParameterName = "@TeacherId";
                    teacherIdParameter.Value = teacherId;
                    command.Parameters.Add(teacherIdParameter);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var course = new Course
                        {
                            CourseCode = reader["course_code"].ToString(),
                            CourseName = reader["course_name"].ToString()
                        };
                        courses.Add(course);
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

            return courses;
        }
    }
}
