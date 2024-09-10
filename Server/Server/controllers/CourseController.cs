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

            [JsonPropertyName("courseId")]
            public string CourseId { get; set; }
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

        public class Lesson
        {
            [JsonPropertyName("lessonId")]
            public string LessonId { get; set; }

            [JsonPropertyName("lessonName")]
            public string LessonName { get; set; }

            [JsonPropertyName("lessonDate")]
            public string LessonDate { get; set; }
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
                        SELECT course_code, course_name ,course_id
                        FROM courses
                        WHERE teacher_id = @teacherId";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@teacherId";
                    parameter.Value = teacherId;
                    command.Parameters.Add(parameter);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var course = new Course
                        {
                            CourseCode = reader["course_code"]?.ToString() ?? "Unknown",
                            CourseName = reader["course_name"]?.ToString() ?? "Unknown",
                            CourseId = reader["course_id"]?.ToString() ?? "Unknown",

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

        public static List<Student> GetStudentsByCourse(string courseId)
        {
            var students = new List<Student>();

            using var db = new Database(DatabaseType.SQLite);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        SELECT u.first_name, u.last_name, u.user_id
                        FROM enrollments e
                        JOIN users u ON e.student_id = u.user_id
                        WHERE e.course_id = @courseId";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@courseId";
                    parameter.Value = courseId;
                    command.Parameters.Add(parameter);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var student = new Student
                        {
                            FirstName = reader["first_name"].ToString() ?? "Unknown",
                            LastName = reader["last_name"].ToString() ?? "Unknown",
                            UserId = reader["user_id"].ToString() ?? "Unknown",
                        };
                        students.Add(student);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error fetching students: {ex.Message}");
                }
                finally
                {
                    db.Close_Connection();
                }
            }

            return students;
        }

        public static List<Lesson> GetAllLessonsForCourse(string courseId)
        {
            var lessons = new List<Lesson>();

            using var db = new Database(DatabaseType.SQLite);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        SELECT lesson_id, lesson_name, lesson_date
                        FROM lessons                     
                        WHERE course_id = @courseId"; 

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@courseId";
                    parameter.Value = courseId;
                    command.Parameters.Add(parameter);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var lesson = new Lesson
                        {
                            LessonId = reader["lesson_id"].ToString() ?? "Unknown",
                            LessonName = reader["lesson_name"].ToString() ?? "Unknown",
                            LessonDate = reader["lesson_date"].ToString() ?? "Unknown", 
                        };
                        lessons.Add(lesson);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error fetching lessons: {ex.Message}"); 
                }
                finally
                {
                    db.Close_Connection();
                }
            }

            return lessons;
        }
    }
}
