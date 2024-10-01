using System.Net;
using System.Text;

namespace WebServer.Server.routes
{
    internal class BaseApi
    {
        protected static async Task WriteResponseAsync(HttpListenerContext context, string responseString, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;

            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.OutputStream.Close();
        }

        protected static void LogError(string message)
        {
            Console.WriteLine($"Error: {message}");
        }
    }
}
