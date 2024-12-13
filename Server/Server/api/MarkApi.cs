using System.Net;
using System.Text.Json;
using HttpServer.Server.controllers;
using HttpServer.Server.utility;

namespace HttpServer.Server.routes
{
    internal class MarkApi : BaseApi
    {
        public static async Task HandleAsync(HttpListenerContext context)
        {
            string responseString = "";
            int statusCode = 200;

            try
            {
                var requestUrl = context.Request.Url.AbsolutePath;
                var httpMethod = context.Request.HttpMethod;

                using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                var body = await reader.ReadToEndAsync();

                var formDataParser = FormDataParser.Parse(body);

                responseString = httpMethod switch
                {
                    "GET" when requestUrl == "/api/lesson/student/marks" => HandleGetMarks(formDataParser),
                    "PUT" when requestUrl == "/api/lesson/teacher/update/marks" => HandleUpdateMarkAsTeacher(formDataParser),
                    "PUT" when requestUrl == "/api/lesson/student/update/studentmarks" => HandleUpdateMarkAsStudent(formDataParser),
                    _ => JsonSerializer.Serialize(new { message = "Endpoint not found." })
                };

                if (responseString.Contains("Endpoint not found."))
                {
                    statusCode = 404;
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

        private static string HandleGetMarks(FormDataParser formDataParser)
        {
            using var markController = new MarkController();

            if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("lessonId"))
            {
                string studentId = formDataParser.GetValue("studentId");
                string lessonId = formDataParser.GetValue("lessonId");

                var marks = markController.GetMarksForLessons(studentId, lessonId);

                return marks != null && marks.Count > 0
                    ? JsonSerializer.Serialize(marks)
                    : JsonSerializer.Serialize(new { message = "No marks found for the given UserId and LessonId." });
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }

        private static string HandleUpdateMarkAsTeacher(FormDataParser formDataParser)
        {
            using var markController = new MarkController();

            if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("teacherId") &&
                formDataParser.ContainsKey("lessonId") && formDataParser.ContainsKey("teacherMark") &&
                formDataParser.ContainsKey("finalMark"))
            {
                string studentId = formDataParser.GetValue("studentId");
                string teacherId = formDataParser.GetValue("teacherId");
                string lessonId = formDataParser.GetValue("lessonId");
                string teacherMark = formDataParser.GetValue("teacherMark");
                string finalMark = formDataParser.GetValue("finalMark");

                var message = markController.UpdateMarkAsTeacher(studentId, teacherId, lessonId, teacherMark, finalMark);

                return message != null
                    ? JsonSerializer.Serialize(message)
                    : JsonSerializer.Serialize(new { message = "Failed to update mark for the given UserId and LessonId." });
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }

        private static string HandleUpdateMarkAsStudent(FormDataParser formDataParser)
        {
            using var markController = new MarkController();

            if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("lessonId") &&
                formDataParser.ContainsKey("studentMark"))
            {
                string studentId = formDataParser.GetValue("studentId");
                string lessonId = formDataParser.GetValue("lessonId");
                string studentMark = formDataParser.GetValue("studentMark");
                var message = markController.UpdateMarkAsStudent(studentId, lessonId, studentMark);

                return message != null
                    ? JsonSerializer.Serialize(message)
                    : JsonSerializer.Serialize(new { message = "Failed to update mark for the given UserId and LessonId." });
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }
    }
}
