using System.Net;
using System.Text.Json;
using HttpServer.Server.controllers;
using HttpServer.Server.utility;

namespace HttpServer.Server.routes
{
    internal class AuthApi : BaseApi
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

                        // Correct the use of the 'AuthController' instance
                        using var authController = new AuthController();
                        var (isAuthenticated, uid, role, firstName, lastName) = authController.AuthenticateUser(username, password);

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
                LogError(ex.Message);
                responseString = "Internal Server Error";
                statusCode = 500;
            }

            await WriteResponseAsync(context, responseString, statusCode);
        }
    }
}
