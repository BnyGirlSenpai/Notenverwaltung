using Server.Server.controllers;
using Server.Server.utility;
using System.Net;
using System.Text;

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
                if (context.Request.HttpMethod == "POST" && context.Request.Url.AbsolutePath == "/api/user/marks")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();
                    var formDataParser = FormDataParser.Parse(body);

                    if (formDataParser.ContainsKey("userId") && formDataParser.ContainsKey("lessonId"))
                    {
                        string userId = formDataParser.GetValue("userId");
                        string lessonId = formDataParser.GetValue("lessonId");

                        var (MarkId, StudentMark, TeacherMark, EndMark, TeacherName) = MarkController.GetMarksForLessons(userId, lessonId);                      
                    }
                    else
                    {
                        responseString = "Invalid request data.";
                        statusCode = 400;
                    }
                }
                else
                {
                    responseString = "Endpoint not found.";
                    statusCode = 404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
                responseString = "Internal Server Error";
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
