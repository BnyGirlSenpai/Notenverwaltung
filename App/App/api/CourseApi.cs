using App.App.controller;
using App.App.repositorys;

namespace App.App.api
{
    internal class CourseApi : BaseApi
    {
        public static async Task<List<CourseRepository>> GetAllCoursesForTeacher(string userId, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalCourseController.GetCoursesByTeacherAsync(userId);
            }
            else
            {
                string responseData = await SendGetRequest("http://localhost:5000/api/teacher/courses",
                    new Dictionary<string, string>
                    {
                        { "userId", userId }
                    });

                return await DeserializeJsonAsync<CourseRepository>(responseData);
            }
        }

        public static async Task<List<CourseRepository>> GetAllCoursesForStudent(string userId, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalCourseController.GetCoursesByStudentAsync(userId);
            }
            else
            {
                string responseData = await SendGetRequest("http://localhost:5000/api/student/courses",
                    new Dictionary<string, string>
                    {
                        { "userId", userId }
                    });

                return await DeserializeJsonAsync<CourseRepository>(responseData);
            }
        }

        public static async Task<List<StudentRepository>> GetAllStudentsForCourse(string userId, string courseId, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalCourseController.GetStudentsByCourseAsync(courseId);
            }
            else
            {
                string responseData = await SendGetRequest("http://localhost:5000/api/teacher/students",
                    new Dictionary<string, string>
                    {
                        { "userId", userId },
                        { "courseId", courseId }
                    });

                return await DeserializeJsonAsync<StudentRepository>(responseData);
            }
        }

        public static async Task<List<LessonRepository>> GetAllLessonsForCourse(string userId, string courseId, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalCourseController.GetAllLessonsForCourseAsync(courseId);
            }
            else
            {
                string responseData = await SendGetRequest("http://localhost:5000/api/teacher/lessons",
                    new Dictionary<string, string>
                    {
                        { "userId", userId },
                        { "courseId", courseId }
                    });

                return await DeserializeJsonAsync<LessonRepository>(responseData);
            }
        }
    }
}
