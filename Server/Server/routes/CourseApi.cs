using NotenverwaltungsApp.Server.controllers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Server.Server.routes
{
    internal class CourseApi
    {
        public static async Task HandleAsync(HttpListenerContext context)
        {
            string responseString = "";
            int statusCode = 200;

            try
            {
                if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/courses")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();
                    var formData = ParseFormData(body);

                    if (formData.TryGetValue("userId", out string userId))
                    {
                        var courses = CourseController.GetCoursesByTeacher(userId);

                        if (courses != null && courses.Count > 0)
                        {
                            responseString = JsonSerializer.Serialize(courses);
                        }
                        else
                        {
                            responseString = JsonSerializer.Serialize(new { message = "No courses found for the given UserId." });
                            statusCode = 404;
                        }
                    }
                    else
                    {
                        responseString = JsonSerializer.Serialize(new { message = "Invalid request data." });
                        statusCode = 400;
                    }
                }

                else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/students")
                {

                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();

                    var formData = ParseFormData(body);

                    if (formData.TryGetValue("courseId", out string courseId))
                    {

                        var students = CourseController.GetStudentsByCourse(courseId);

                        if (students != null && students.Count > 0)
                        {
                            responseString = JsonSerializer.Serialize(students);
                        }
                        else
                        {
                            responseString = JsonSerializer.Serialize(new { message = "No users found." });
                            statusCode = 404;
                        }
                    }
                    else
                    {
                        responseString = JsonSerializer.Serialize(new { message = "Invalid request data." });
                        statusCode = 400;
                    }
                }

                else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/lessons")
                {

                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();

                    var formData = ParseFormData(body);

                    if (formData.TryGetValue("courseId", out string courseId))
                    {

                        var lessons = CourseController.GetAllLessonsForCourse(courseId);

                        if (lessons != null && lessons.Count > 0)
                        {
                            responseString = JsonSerializer.Serialize(lessons);
                        }
                        else
                        {
                            responseString = JsonSerializer.Serialize(new { message = "No users found." });
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

        private static Dictionary<string, string> ParseFormData(string body)
        {
            var formData = new Dictionary<string, string>();
            var keyValuePairs = body.Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in keyValuePairs)
            {
                var keyValue = pair.Split('=', 2);
                if (keyValue.Length == 2)
                {
                    formData[Uri.UnescapeDataString(keyValue[0])] = Uri.UnescapeDataString(keyValue[1]);
                }
            }

            return formData;
        }
    }
}
