using Server.Server.routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

                // Handle other routes here...

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
