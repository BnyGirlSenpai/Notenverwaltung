using App.App.repositorys;
using System.Data.SQLite;

namespace App.App.controller
{
    internal class LocalCourseController
    {
        public static async Task<List<CourseRepository>> GetCoursesByTeacherAsync(string teacherId)
        {
            try
            {
                using var connection = new SQLiteConnection("Data Source=C:\\Users\\drebes\\Berufsschule\\SDM\\SQL\\Database\\Notenverwaltung.db3;Version=3;");
                await connection.OpenAsync();

                var command = new SQLiteCommand("SELECT course_code, course_name, course_id FROM courses WHERE teacher_id = @teacherId", connection);
                command.Parameters.AddWithValue("@teacherId", teacherId);

                var courses = new List<CourseRepository>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var course = new CourseRepository
                        {
                            CourseCode = reader["course_code"]?.ToString() ?? "Unknown",
                            CourseName = reader["course_name"]?.ToString() ?? "Unknown",
                            CourseId = reader["course_id"]?.ToString() ?? "Unknown",
                        };
                        courses.Add(course);
                    }
                }

                return courses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching courses: {ex.Message}");
                return []; 
            }
        }

        public static async Task<List<CourseRepository>> GetCoursesByStudentAsync(string studentId)
        {
            try
            {
                using var connection = new SQLiteConnection("Data Source=C:\\Users\\drebes\\Berufsschule\\SDM\\SQL\\Database\\Notenverwaltung.db3;Version=3;");
                await connection.OpenAsync();

                var command = new SQLiteCommand("SELECT e.course_id, c.course_name, c.course_code FROM enrollments e LEFT JOIN courses c ON e.course_id = c.course_id WHERE e.student_id = @studentId", connection);
                command.Parameters.AddWithValue("@studentId", studentId);

                var courses = new List<CourseRepository>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var course = new CourseRepository
                        {
                            CourseCode = reader["course_code"]?.ToString() ?? "Unknown",
                            CourseName = reader["course_name"]?.ToString() ?? "Unknown",
                            CourseId = reader["course_id"]?.ToString() ?? "Unknown",
                        };
                        courses.Add(course);
                    }
                }

                return courses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching courses: {ex.Message}");
                return [];
            }
        }

        public static async Task<List<StudentRepository>> GetStudentsByCourseAsync(string courseId)
        {
            try
            {
                using var connection = new SQLiteConnection("Data Source=C:\\Users\\drebes\\Berufsschule\\SDM\\SQL\\Database\\Notenverwaltung.db3;Version=3;");
                await connection.OpenAsync();

                var command = new SQLiteCommand("SELECT u.first_name, u.last_name, u.user_id FROM enrollments e JOIN users u ON e.student_id = u.user_id WHERE e.course_id = @courseId", connection);  
                command.Parameters.AddWithValue("@courseId", courseId);

                var students = new List<StudentRepository>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var student = new StudentRepository
                        {
                            FirstName = reader["first_name"].ToString() ?? "Unknown",
                            LastName = reader["last_name"].ToString() ?? "Unknown",
                            UserId = reader["user_id"].ToString() ?? "Unknown",
                        };
                        students.Add(student);
                    }
                }

                return students;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching courses: {ex.Message}");
                return [];
            }
        }

        public static async Task<List<LessonRepository>> GetAllLessonsForCourseAsync(string courseId)
        {
            try
            {
                using var connection = new SQLiteConnection("Data Source=C:\\Users\\drebes\\Berufsschule\\SDM\\SQL\\Database\\Notenverwaltung.db3;Version=3;");
                await connection.OpenAsync();

                var command = new SQLiteCommand("SELECT lesson_id, lesson_name, lesson_date, lesson_type FROM lessons WHERE course_id = @courseId", connection);
                command.Parameters.AddWithValue("@courseId", courseId);

                var lessons = new List<LessonRepository>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lesson = new LessonRepository
                        {
                            LessonId = reader["lesson_id"].ToString() ?? "Unknown",
                            LessonName = reader["lesson_name"].ToString() ?? "Unknown",
                            LessonDate = reader["lesson_date"].ToString() ?? "Unknown",
                            LessonType = reader["lesson_type"].ToString() ?? "Unknown",
                        };
                        lessons.Add(lesson);
                    }
                }

                return lessons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching courses: {ex.Message}");
                return [];
            }
        }
    }
}
