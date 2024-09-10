using NotenverwaltungsApp.Server.controllers;
using Server.Server.utility;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Server.Server.routes
{
    internal class AttendanceApi
    {
        public static async Task HandleAsync(HttpListenerContext context)
        {
            string responseString = "";
            int statusCode = 200;

            try
            {           
                if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/lesson/student/attendance")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();

                    var formDataParser = FormDataParser.Parse(body);


                    if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("lessonId"))
                    {
                        string studentId = formDataParser.GetValue("studentId");
                        string lessonId = formDataParser.GetValue("lessonId");

                        var attendances = AttendanceController.GetAttendanceForLesson(studentId, lessonId);


                        if (attendances != null && attendances.Count > 0)
                        {
                            responseString = JsonSerializer.Serialize(attendances);
                        }
                        else
                        {
                            responseString = JsonSerializer.Serialize(new { message = "No marks found for the given UserId and LessonId." });
                            statusCode = 404;
                        }
                    }
                    else
                    {
                        responseString = JsonSerializer.Serialize(new { message = "Invalid request data." });
                        statusCode = 400;
                    }
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
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}
