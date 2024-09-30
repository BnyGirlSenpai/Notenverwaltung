using App.App.repositorys;
using System.Data.SQLite;

namespace App.App.controller
{
    internal class LocalCourseController : LocalBaseController
    {
        public static async Task<List<CourseRepository>> GetCoursesByTeacherAsync(string teacherId)
        {
            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand("SELECT course_code, course_name, course_id FROM courses WHERE teacher_id = @teacherId", connection);
            command.Parameters.AddWithValue("@teacherId", teacherId);

            return await ExecuteQueryAsync(command, async reader => new CourseRepository
            {
                CourseCode = reader["course_code"]?.ToString() ?? "Unknown",
                CourseName = reader["course_name"]?.ToString() ?? "Unknown",
                CourseId = reader["course_id"]?.ToString() ?? "Unknown"
            });
        }

        public static async Task<List<CourseRepository>> GetCoursesByStudentAsync(string studentId)
        {
            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand("SELECT e.course_id, c.course_name, c.course_code FROM enrollments e LEFT JOIN courses c ON e.course_id = c.course_id WHERE e.student_id = @studentId", connection);
            command.Parameters.AddWithValue("@studentId", studentId);

            return await ExecuteQueryAsync(command, async reader => new CourseRepository
            {
                CourseCode = reader["course_code"]?.ToString() ?? "Unknown",
                CourseName = reader["course_name"]?.ToString() ?? "Unknown",
                CourseId = reader["course_id"]?.ToString() ?? "Unknown"
            });
        }

        public static async Task<List<StudentRepository>> GetStudentsByCourseAsync(string courseId)
        {
            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand("SELECT u.first_name, u.last_name, u.user_id FROM enrollments e JOIN users u ON e.student_id = u.user_id WHERE e.course_id = @courseId", connection);
            command.Parameters.AddWithValue("@courseId", courseId);

            return await ExecuteQueryAsync(command, async reader => new StudentRepository
            {
                FirstName = reader["first_name"].ToString() ?? "Unknown",
                LastName = reader["last_name"].ToString() ?? "Unknown",
                UserId = reader["user_id"].ToString() ?? "Unknown"
            });
        }

        public static async Task<List<LessonRepository>> GetAllLessonsForCourseAsync(string courseId)
        {
            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand("SELECT lesson_id, lesson_name, lesson_date, lesson_type FROM lessons WHERE course_id = @courseId", connection);
            command.Parameters.AddWithValue("@courseId", courseId);

            return await ExecuteQueryAsync(command, async reader => new LessonRepository
            {
                LessonId = reader["lesson_id"].ToString() ?? "Unknown",
                LessonName = reader["lesson_name"].ToString() ?? "Unknown",
                LessonDate = reader["lesson_date"].ToString() ?? "Unknown",
                LessonType = reader["lesson_type"].ToString() ?? "Unknown"
            });
        }
    }
}
