using System.Net;
using System.Text;
using WebServer.Server.routes;

namespace WebServer.Server.handler
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

                case "/api/teacher/courses":
                    await CourseApi.HandleAsync(context);
                    break;

                case "/api/student/courses":
                    await CourseApi.HandleAsync(context);
                    break;

                case "/api/teacher/students":
                    await CourseApi.HandleAsync(context);
                    break;

                case "/api/teacher/lessons":
                    await CourseApi.HandleAsync(context);
                    break;

                case "/api/lesson/student/marks":
                    await MarkApi.HandleAsync(context);
                    break;

                case "/api/lesson/teacher/update/marks":
                    await MarkApi.HandleAsync(context);
                    break;

                case "/api/lesson/student/update/studentmarks":
                    await MarkApi.HandleAsync(context);
                    break;

                case "/api/lesson/student/attendance":
                    await AttendanceApi.HandleAsync(context);
                    break;

                case "/api/lesson/student/update/attendance":
                    await AttendanceApi.HandleAsync(context);
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
