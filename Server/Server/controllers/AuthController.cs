using static NotenverwaltungsApp.Database;

namespace NotenverwaltungsApp.Server.controllers
{
    internal class AuthController
    {
        public static (bool isAuthenticated, string userId, string userRole, string firstName, string lastName) AuthenticateUser(string email, string password)
        {
            bool isAuthenticated = false;
            string userId = null;
            string userRole = null;
            string firstName = null;
            string lastName = null;


            using var db = new Database(DatabaseType.SQLite);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        SELECT users.user_id, users.first_name, users.last_name, roles.role_name
                        FROM users
                        LEFT JOIN roles ON users.role_id = roles.role_id
                        WHERE users.email = @Email AND users.password = @Password";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var emailParameter = command.CreateParameter();
                    emailParameter.ParameterName = "@Email";
                    emailParameter.Value = email;
                    command.Parameters.Add(emailParameter);

                    var passwordParameter = command.CreateParameter();
                    passwordParameter.ParameterName = "@Password";
                    passwordParameter.Value = password;
                    command.Parameters.Add(passwordParameter);

                    using var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        isAuthenticated = true;
                        userId = reader["user_id"].ToString();
                        firstName = reader["first_name"].ToString();
                        lastName = reader["last_name"].ToString();
                        userRole = reader["role_name"].ToString();
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
            }

            return (isAuthenticated, userId, userRole, firstName, lastName);
        }
    }
}
