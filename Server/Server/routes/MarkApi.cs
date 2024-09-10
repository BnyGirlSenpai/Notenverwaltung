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
                // Log the incoming request details
                Console.WriteLine($"Incoming request: {context.Request.HttpMethod} {context.Request.Url.AbsolutePath}");

                if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/lesson/student/marks")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();

                    // Log the request body
                    Console.WriteLine($"Request body: {body}");

                    var formDataParser = FormDataParser.Parse(body);


                    if (formDataParser.ContainsKey("studentId") && formDataParser.ContainsKey("lessonId"))
                    {
                        string studentId = formDataParser.GetValue("studentId");
                        string lessonId = formDataParser.GetValue("lessonId");

                        // Log extracted parameters
                        Console.WriteLine($"StudentId: {studentId}");
                        Console.WriteLine($"LessonId: {lessonId}");

                        var marks = MarkController.GetMarksForLessons(studentId, lessonId);

                        // Log the results from the controller

                        if (marks != null && marks.Count > 0)
                        {
                            responseString = JsonSerializer.Serialize(marks);
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
                // Log the exception details
                Console.WriteLine($"Error handling request: {ex.Message}");
                responseString = JsonSerializer.Serialize(new { message = "Internal Server Error" });
                statusCode = 500;
            }

            // Log the response status code and content length
            Console.WriteLine($"Response status code: {statusCode}");
            Console.WriteLine($"Response content length: {Encoding.UTF8.GetByteCount(responseString)}");

            context.Response.StatusCode = statusCode;
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}
