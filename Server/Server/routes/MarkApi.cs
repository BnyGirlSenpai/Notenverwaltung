using NotenverwaltungsApp.Server.controllers;
using Server.Server.utility;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Server.Server.routes
{
    internal class MarkApi
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

                if (httpMethod == "GET" && requestUrl == "/api/lesson/student/marks")
                {
                    responseString = HandleGetMarks(formDataParser);
                }
                else if (httpMethod == "PUT" && requestUrl == "/api/lesson/student/update/marks")
                {
                    responseString = HandleUpdateMarkAsTeacher(formDataParser);
                }
                else if (httpMethod == "PUT" && requestUrl == "/api/lesson/student/update/studentmarks")
                {
                    responseString = HandleUpdateMarkAsStudent(formDataParser);
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { message = "Endpoint not found." });
                    statusCode = 404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
                responseString = JsonSerializer.Serialize(new { message = "Internal Server Error" });
                statusCode = 500;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }

        private static string HandleGetMarks(FormDataParser formDataParser)
        {
            if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("lessonId"))
            {
                string studentId = formDataParser.GetValue("studentId");
                string lessonId = formDataParser.GetValue("lessonId");

                var marks = MarkController.GetMarksForLessons(studentId, lessonId);

                if (marks != null && marks.Count > 0)
                {
                    return JsonSerializer.Serialize(marks);
                }
                else
                {
                    return JsonSerializer.Serialize(new { message = "No marks found for the given UserId and LessonId." });
                }
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }

        private static string HandleUpdateMarkAsTeacher(FormDataParser formDataParser)
        {
            if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("teacherId") && formDataParser.ContainsKey("lessonId") && formDataParser.ContainsKey("teacherMark") && formDataParser.ContainsKey("finalMark"))
            {
                string studentId = formDataParser.GetValue("studentId");
                string teacherId = formDataParser.GetValue("teacherId");
                string lessonId = formDataParser.GetValue("lessonId");
                string teacherMark = formDataParser.GetValue("teacherMark");
                string finalMark = formDataParser.GetValue("finalMark");

                var message = MarkController.UpdateMarkAsTeacher(studentId, teacherId, lessonId, teacherMark, finalMark);

                if (message != null)
                {
                    return JsonSerializer.Serialize(message);
                }
                else
                {
                    return JsonSerializer.Serialize(new { message = "Failed to update mark for the given UserId and LessonId." });
                }
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }

        private static string HandleUpdateMarkAsStudent(FormDataParser formDataParser)
        {
            if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("lessonId") && formDataParser.ContainsKey("studentMark"))
            {
                string studentId = formDataParser.GetValue("studentId");
                string lessonId = formDataParser.GetValue("lessonId");
                string studentMark = formDataParser.GetValue("studentMark");
                var message = MarkController.UpdateMarkAsStudent(studentId, lessonId, studentMark);

                if (message != null)
                {
                    Console.WriteLine(message);
                    return JsonSerializer.Serialize(message);
                }
                else
                {
                    return JsonSerializer.Serialize(new { message = "Failed to update mark for the given UserId and LessonId." });
                }
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }
    }
}
