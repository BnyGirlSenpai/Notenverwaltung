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

        public class Student
        {
            [JsonPropertyName("firstname")]
            public string FirstName { get; set; }
            [JsonPropertyName("lastname")]
            public string LastName { get; set; }

            [JsonPropertyName("userId")]
            public string UserId { get; set; }
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
                            CourseCode = reader["course_code"].ToString() ?? "Unknown",
                            CourseName = reader["course_name"].ToString() ?? "Unknown",
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

        public static List<Student> GetStudentsByCourse(string courseCode)
        {
            var students = new List<Student>();

            using var db = new Database(DatabaseType.SQLite);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        SELECT u.firstname, u.lastname, u.user_id
                        FROM enrollments e
                        JOIN users u ON e.student_id = u.user_id
                        WHERE e.course_code = @CourseCode";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var courseCodeParameter = command.CreateParameter();
                    courseCodeParameter.ParameterName = "@CourseCode";
                    courseCodeParameter.Value = courseCode;
                    command.Parameters.Add(courseCodeParameter);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var student = new Student
                        {
                            FirstName = reader["firstname"].ToString() ?? "Unknown",
                            LastName = reader["lastname"].ToString() ?? "Unknown",
                            UserId = reader["user_id"].ToString() ?? "Unknown",
                        };
                        students.Add(student);
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

            return students;
        }

    }
}
