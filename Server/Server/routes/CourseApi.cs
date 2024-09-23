using NotenverwaltungsApp.Server.controllers;
using Server.Server.utility;
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
                if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/teacher/courses")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();
                    var formDataParser = FormDataParser.Parse(body);

                    if (formDataParser.ContainsKey("userId"))
                    {
                        string userId = formDataParser.GetValue("userId");
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

                else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/teacher/students")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();
                    var formDataParser = FormDataParser.Parse(body);

                    if (formDataParser.ContainsKey("courseId"))
                    {
                        string courseId = formDataParser.GetValue("courseId");
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

                else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/teacher/lessons")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();
                    var formDataParser = FormDataParser.Parse(body);

                    if (formDataParser.ContainsKey("courseId"))
                    {
                        string courseId = formDataParser.GetValue("courseId");
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

                else if (context.Request.HttpMethod == "GET" && context.Request.Url.AbsolutePath == "/api/student/courses")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();
                    var formDataParser = FormDataParser.Parse(body);

                    if (formDataParser.ContainsKey("userId"))
                    {
                        string userId = formDataParser.GetValue("userId");
                        var courses = CourseController.GetCoursesByStudent(userId);

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
