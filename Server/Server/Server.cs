using Server.Server.handler;
using System.Net;

namespace NotenverwaltungsApp.Server
{
    internal class Server
    {
        private readonly HttpListener _listener;
        private readonly RequestHandler _requestHandler;

        public Server()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:5000/");
            _requestHandler = new RequestHandler(); 
        }

        public async Task StartServerAsync()
        {
            if (!TestDatabaseConnection())
            {
                Console.WriteLine("Failed to connect to the database. Shutting down server.");
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
                await RequestHandler.HandleRequestAsync(context); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
            }
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
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    using var command = connection.CreateCommand();
                    {
                        command.CommandText = "SELECT datetime('now')";
                        var result = command.ExecuteScalar();
                        Console.WriteLine($"Current date and time from database: {result}");

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
