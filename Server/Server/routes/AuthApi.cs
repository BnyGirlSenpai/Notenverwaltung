using System.Net;
using System.Text;
using System.Text.Json;
using WebServer.Server.controllers;
using WebServer.Server.utility;

namespace WebServer.Server.routes
{
    internal class AuthApi
    {
        public static async Task HandleAsync(HttpListenerContext context)
        {
            string responseString = "";
            int statusCode = 200;

            try
            {
                if (context.Request.HttpMethod == "POST" && context.Request.Url.AbsolutePath == "/api/auth/login")
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    var body = await reader.ReadToEndAsync();
                    var formDataParser = FormDataParser.Parse(body);

                    if (formDataParser.ContainsKey("username") && formDataParser.ContainsKey("password"))
                    {
                        string username = formDataParser.GetValue("username");
                        string password = formDataParser.GetValue("password");

                        var (isAuthenticated, uid, role, firstName, lastName) = AuthController.AuthenticateUser(username, password);

                        if (isAuthenticated)
                        {
                            var userInfo = new
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                UserId = uid,
                                Role = role,
                                Message = "User authenticated successfully."
                            };

                            responseString = JsonSerializer.Serialize(userInfo);
                        }
                        else
                        {
                            responseString = "Invalid username or password.";
                            statusCode = 401;
                        }
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
