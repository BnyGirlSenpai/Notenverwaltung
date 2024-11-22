using WebServer.Server.repositorys;

namespace WebServer.Server.controllers
{
    internal class CourseController : BaseController, IDisposable
    {
        public CourseController() 
        {
            ConnectToDatabase();
        }

        public List<CourseRepository> GetCoursesByTeacher(string teacherId)
        {
            string query = @"
                SELECT course_code, course_name ,course_id
                FROM courses
                WHERE teacher_id = @teacherId";

            using var command = CreateCommand(query);
            AddParameter(command, "@teacherId", teacherId);

            var courses = ExecuteReader(command, reader => new CourseRepository
            {
                CourseCode = reader["course_code"]?.ToString() ?? "Unknown",
                CourseName = reader["course_name"]?.ToString() ?? "Unknown",
                CourseId = reader["course_id"]?.ToString() ?? "Unknown",
            });

            return courses;
        }

        public List<CourseRepository> GetCoursesByStudent(string studentId)
        {
            string query = @"
                SELECT e.course_id, c.course_name, c.course_code
                FROM enrollments e
                LEFT JOIN courses c ON e.course_id = c.course_id
                WHERE e.student_id = @studentId";

            using var command = CreateCommand(query);
            AddParameter(command, "@studentId", studentId);

            var courses = ExecuteReader(command, reader => new CourseRepository
            {
                CourseCode = reader["course_code"]?.ToString() ?? "Unknown",
                CourseName = reader["course_name"]?.ToString() ?? "Unknown",
                CourseId = reader["course_id"]?.ToString() ?? "Unknown",
            });

            return courses;
        }

        public List<StudentRepository> GetStudentsByCourse(string courseId)
        {
            string query = @"
                SELECT u.first_name, u.last_name, u.user_id
                FROM enrollments e
                JOIN users u ON e.student_id = u.user_id
                WHERE e.course_id = @courseId";

            using var command = CreateCommand(query);
            AddParameter(command, "@courseId", courseId);

            var students = ExecuteReader(command, reader => new StudentRepository
            {
                FirstName = reader["first_name"].ToString() ?? "Unknown",
                LastName = reader["last_name"].ToString() ?? "Unknown",
                UserId = reader["user_id"].ToString() ?? "Unknown",
            });

            return students;
        }

        public List<LessonRepository> GetAllLessonsForCourse(string courseId)
        {
            string query = @"
                SELECT lesson_id, lesson_name, lesson_date, lesson_type
                FROM lessons                     
                WHERE course_id = @courseId";

            using var command = CreateCommand(query);
            AddParameter(command, "@courseId", courseId);

            var lessons = ExecuteReader(command, reader => new LessonRepository
            {
                LessonId = reader["lesson_id"].ToString() ?? "Unknown",
                LessonName = reader["lesson_name"].ToString() ?? "Unknown",
                LessonDate = reader["lesson_date"].ToString() ?? "Unknown",
                LessonType = reader["lesson_type"].ToString() ?? "Unknown",
            });

            return lessons;
        }

        public void Dispose()
        {
            CloseConnection();
        }   
    }
}
