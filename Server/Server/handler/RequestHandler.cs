using Server.Server.routes;
using System.Net;
using System.Text;

namespace Server.Server.handler
{
    internal class RequestHandler
    {
        public static async Task HandleRequestAsync(HttpListenerContext context)
        {
            switch (context.Request.Url.AbsolutePath)
            {
                case "/api/auth/login":
                    await AuthApi.HandleAsync(context);
                    break;

                case "/api/courses":
                    await CourseApi.HandleAsync(context);
                    break;

                case "/api/students":
                    await CourseApi.HandleAsync(context);
                    break;

                case "/api/lessons":
                    await CourseApi.HandleAsync(context);
                    break;

                case "/api/lesson/student/marks":
                    await MarkApi.HandleAsync(context);
                    break;

                default:
                    string responseString = "Endpoint not found.";
                    int statusCode = 404; 

                    context.Response.StatusCode = statusCode;
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    context.Response.OutputStream.Close();
                    break;
            }
        }
    }
}
