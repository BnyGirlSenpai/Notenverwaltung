using NotenverwaltungsApp.Server.controllers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Server.Server.routes
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
                    var formData = ParseFormData(body);

                    if (formData.TryGetValue("username", out string username) && formData.TryGetValue("password", out string password))
                    {
                        var (isAuthenticated, uid, role, firstName ,lastName) = AuthController.AuthenticateUser(username, password);

                        if (isAuthenticated)
                        {
                            Console.WriteLine($"User authenticated successfully.");
                            var userInfo = new
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                UserId = uid,
                                Role = role,
                                Message = "User authenticated successfully."
                            };

                            responseString = JsonSerializer.Serialize(userInfo);
                            Console.WriteLine(responseString);
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
