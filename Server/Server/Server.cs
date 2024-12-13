using System.Net;
using HttpServer.Server.config;
using HttpServer.Server.handler;
using HttpServer.Server.services;

namespace HttpServer.Server
{
    internal class Server
    {
        private readonly HttpListener _listener;
        private static readonly string onlineDatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "OnlineNotenverwaltung.db3");

        public Server()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:5000/");
        }

        public async Task StartServerAsync()
        {
            if (!TestDatabaseConnection())
            {
                return;
            }

            _listener.Start();
            Console.WriteLine("HTTP Server started at http://localhost:5000/");

            while (_listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(context));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling request: {ex.Message}");
                }
            }
        }

        public void StopServer()
        {
            _listener.Stop();
            Console.WriteLine("HTTP Server stopped.");
        }

        private static async Task HandleRequestAsync(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;

                if (path == "/ping")
                {
                    await HandlePingRequest(context);
                }
                else
                {
                    await RequestHandler.HandleRequestAsync(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
            }
        }

        private static async Task HandlePingRequest(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/plain";
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes("Pong");
            await context.Response.OutputStream.WriteAsync(responseBytes);
            context.Response.Close();
        }

        static async Task Main()
        {
            var server = new Server();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                Console.WriteLine("Stopping the server...");
                server.StopServer();
            };
            await server.StartServerAsync();
            Console.WriteLine("Press Ctrl+C to stop the server...");
        }

        private static bool TestDatabaseConnection()
        {
            bool connectionSuccess = false;

            using (var db = new Database(Database.DatabaseType.SQLite))
            {
                try
                {
                    OnlineDatabaseService.InitializeOnlineDatabase(onlineDatabasePath);
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    using var command = connection.CreateCommand();
                    {
                        if (db.Type == Database.DatabaseType.SQLite)
                        {
                            command.CommandText = "SELECT CURRENT_TIMESTAMP";
                        }
                        else if (db.Type == Database.DatabaseType.MySQL)
                        {
                            command.CommandText = "SELECT NOW()";
                        }

                        var result = command.ExecuteScalar();
                        Console.WriteLine($"Current date and time from the database: {result}");

                        connectionSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to the database: {ex.Message}");
                }
                finally
                {
                    db.Close_Connection();
                }
            }

            return connectionSuccess;
        }
    }
}
