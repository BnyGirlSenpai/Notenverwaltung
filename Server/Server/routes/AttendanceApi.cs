using System.Net;
using System.Text.Json;
using WebServer.Server.controllers;
using WebServer.Server.utility;

namespace WebServer.Server.routes
{
    internal class AttendanceApi : BaseApi
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

                if (httpMethod == "GET" && requestUrl == "/api/lesson/student/attendance")
                {
                    responseString = HandleGetAttendance(formDataParser);
                }
                else if (httpMethod == "PUT" && requestUrl == "/api/lesson/student/update/attendance")
                {
                    responseString = HandleUpdateAttendance(formDataParser);
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { message = "Endpoint not found." });
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

        private static string HandleGetAttendance(FormDataParser formDataParser)
        {
            using var attendanceController = new AttendanceController();

            if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("lessonId"))
            {
                string studentId = formDataParser.GetValue("studentId");
                string lessonId = formDataParser.GetValue("lessonId");

                var attendances = attendanceController.GetAttendanceForLesson(studentId, lessonId);

                if (attendances != null && attendances.Count > 0)
                {
                    return JsonSerializer.Serialize(attendances);
                }
                else
                {
                    return JsonSerializer.Serialize(new { message = "No attendances found for the given UserId and LessonId." });
                }
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }

        private static string HandleUpdateAttendance(FormDataParser formDataParser)
        {
            using var attendanceController = new AttendanceController();

            if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("lessonId") && formDataParser.ContainsKey("attendanceStatus"))
            {
                string studentId = formDataParser.GetValue("studentId");
                string lessonId = formDataParser.GetValue("lessonId");
                string attendanceStatus = formDataParser.GetValue("attendanceStatus");
                var message = attendanceController.UpdateAttendanceForLesson(studentId, lessonId, attendanceStatus);

                if (message != null)
                {
                    return JsonSerializer.Serialize(message);
                }
                else
                {
                    return JsonSerializer.Serialize(new { message = "No attendances found for the given UserId and LessonId." });
                }
            }
            return JsonSerializer.Serialize(new { message = "Invalid request data." });
        }
    }
}
