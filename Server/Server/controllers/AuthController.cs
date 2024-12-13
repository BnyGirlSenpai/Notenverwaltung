namespace HttpServer.Server.controllers
{
    internal class AuthController : BaseController, IDisposable
    {
        public AuthController() 
        {
            ConnectToDatabase();
        }

        public (bool isAuthenticated, string userId, string userRole, string firstName, string lastName) AuthenticateUser(string email, string password)
        {
            bool isAuthenticated = false;
            string? userId = null;
            string? userRole = null;
            string? firstName = null;
            string? lastName = null;

            try
            {
                string query = @"
                    SELECT users.user_id, users.first_name, users.last_name, roles.role_name 
                    FROM users
                    LEFT JOIN roles ON users.role_id = roles.role_id
                    WHERE users.email = @Email AND users.password = @Password";

                using var command = CreateCommand(query);              
                AddParameter(command, "@Email", email);
                AddParameter(command, "@Password", password);

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isAuthenticated = true;
                    userId = reader["user_id"]?.ToString() ?? "Unknown";
                    firstName = reader["first_name"]?.ToString() ?? "Unknown";
                    lastName = reader["last_name"]?.ToString() ?? "Unknown";
                    userRole = reader["role_name"]?.ToString() ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during authentication: {ex.Message}");
            }
          
            return (isAuthenticated, userId, userRole, firstName, lastName);
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
