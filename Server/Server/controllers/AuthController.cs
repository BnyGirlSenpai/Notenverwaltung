using System.Data.Common;
using WebServer.Server.config;
using static WebServer.Server.config.Database;

namespace WebServer.Server.controllers
{
    internal class AuthController : BaseController
    {
        public static (bool isAuthenticated, string userId, string userRole, string firstName, string lastName) AuthenticateUser(string email, string password)
        {
            bool isAuthenticated = false;
            string userId = null;
            string userRole = null;
            string firstName = null;
            string lastName = null;

            using var db = new Database(DatabaseType.MySQL);
            try
            {
                db.Connect_to_Database();
                var connection = db.GetConnection();

                string query = @"
                    SELECT users.user_id, users.first_name, users.last_name, roles.role_name
                    FROM users
                    LEFT JOIN roles ON users.role_id = roles.role_id
                    WHERE users.email = @Email AND users.password = @Password";

                using var command = CreateCommandWithParameters((DbConnection)connection, query,
                    new (string ParameterName, object Value)[]
                    {
                        ("@Email", email),
                        ("@Password", password)
                    });

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
            finally
            {
                db.Close_Connection();
            }

            return (isAuthenticated, userId, userRole, firstName, lastName);
        }

        private static DbCommand CreateCommandWithParameters(DbConnection connection, string query, (string ParameterName, object Value)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;

            foreach (var parameter in parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.ParameterName;
                dbParameter.Value = parameter.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }
    }
}
