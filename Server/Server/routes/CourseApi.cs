using System.Net;
using System.Text.Json;
using WebServer.Server.controllers;
using WebServer.Server.utility;

namespace WebServer.Server.routes
{
    internal class CourseApi : BaseApi
    {
        public static async Task HandleAsync(HttpListenerContext context)
        {
            string responseString = "";
            int statusCode = 200;
            var courseController = new CourseController();

            try
            {
                using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                var body = await reader.ReadToEndAsync();
                var formDataParser = FormDataParser.Parse(body);

                switch (context.Request.HttpMethod)
                {
                    case "GET":
                        responseString = await HandleGetRequests(context.Request.Url.AbsolutePath, formDataParser, courseController);
                        break;

                    default:
                        responseString = JsonSerializer.Serialize(new { message = "Endpoint not found." });
                        statusCode = 404;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error handling request: {ex.Message}");
                responseString = JsonSerializer.Serialize(new { message = "Internal Server Error" });
                statusCode = 500;
            }

            await WriteResponseAsync(context, responseString, statusCode);
        }

        private static async Task<string> HandleGetRequests(string requestUrl, FormDataParser formDataParser, CourseController courseController)
        {
            string responseString;

            if (requestUrl == "/api/teacher/courses")
            {
                responseString = GetTeacherCourses(formDataParser, courseController);
            }
            else if (requestUrl == "/api/teacher/students")
            {
                responseString = GetStudentsByCourse(formDataParser, courseController);
            }
            else if (requestUrl == "/api/teacher/lessons")
            {
                responseString = GetLessonsByCourse(formDataParser, courseController);
            }
            else if (requestUrl == "/api/student/courses")
            {
                responseString = GetStudentCourses(formDataParser, courseController);
            }
            else
            {
                responseString = JsonSerializer.Serialize(new { message = "Endpoint not found." });
            }

            return responseString;
        }

        private static string GetTeacherCourses(FormDataParser formDataParser, CourseController courseController)
        {
            if (formDataParser.ContainsKey("userId"))
            {
                string userId = formDataParser.GetValue("userId");
                var courses = courseController.GetCoursesByTeacher(userId);

                return courses != null && courses.Count > 0
                    ? JsonSerializer.Serialize(courses)
                    : JsonSerializer.Serialize(new { message = "No courses found for the given UserId." });
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }

        private static string GetStudentsByCourse(FormDataParser formDataParser, CourseController courseController)
        {
            if (formDataParser.ContainsKey("courseId"))
            {
                string courseId = formDataParser.GetValue("courseId");
                var students = courseController.GetStudentsByCourse(courseId);

                return students != null && students.Count > 0
                    ? JsonSerializer.Serialize(students)
                    : JsonSerializer.Serialize(new { message = "No users found." });
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }

        private static string GetLessonsByCourse(FormDataParser formDataParser, CourseController courseController)
        {
            if (formDataParser.ContainsKey("courseId"))
            {
                string courseId = formDataParser.GetValue("courseId");
                var lessons = courseController.GetAllLessonsForCourse(courseId);

                return lessons != null && lessons.Count > 0
                    ? JsonSerializer.Serialize(lessons)
                    : JsonSerializer.Serialize(new { message = "No users found." });
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }

        private static string GetStudentCourses(FormDataParser formDataParser, CourseController courseController)
        {
            if (formDataParser.ContainsKey("userId"))
            {
                string userId = formDataParser.GetValue("userId");
                var courses = courseController.GetCoursesByStudent(userId);

                return courses != null && courses.Count > 0
                    ? JsonSerializer.Serialize(courses)
                    : JsonSerializer.Serialize(new { message = "No courses found for the given UserId." });
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }
    }
}
