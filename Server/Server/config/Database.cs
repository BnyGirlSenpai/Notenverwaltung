using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SQLite;

namespace WebServer.Server.config
{
    internal class Database : IDisposable
    {
        private IDbConnection _connection;
        private readonly string _connectionString;
        private readonly DatabaseType _dbType;

        public enum DatabaseType
        {
            SQLite,
            MySQL
        }

        public DatabaseType Type { get; private set; }

        public Database(DatabaseType dbType)
        {
            _dbType = dbType;
            Type = dbType; 

            _connectionString = _dbType switch
            {
                DatabaseType.SQLite => "Data Source=C:\\Users\\drebes\\Berufsschule\\SDM\\MyProjects\\Notenverwaltung\\Database\\OnlineNotenverwaltung.db3 ",
                DatabaseType.MySQL => BuildMySqlConnectionString(),
                _ => throw new NotSupportedException("Database type not supported."),
            };
        }

        private static string BuildMySqlConnectionString()
        {
            string host = "localhost";
            string user = "root";
            string password = "password";
            string database = "notenverwaltung";

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database))
            {
                throw new InvalidOperationException("One or more environment variables for the MySQL connection are missing.");
            }

            return $"Server={host};Database={database};Uid={user};Pwd={password};";
        }

        public void Connect_to_Database()
        {
            try
            {
                _connection = _dbType switch
                {
                    DatabaseType.SQLite => new SQLiteConnection(_connectionString),
                    DatabaseType.MySQL => new MySqlConnection(_connectionString),
                    _ => throw new NotSupportedException("Database type not supported."),
                };
                _connection.Open();
                Console.WriteLine("Database connection established successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error establishing database connection: {ex.Message}");
                throw;
            }
        }

        public void Close_Connection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                try
                {
                    _connection.Close();
                    Console.WriteLine("Database connection closed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error closing database connection: {ex.Message}");
                }
            }
        }

        public IDbConnection GetConnection()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Connection is not initialized.");
            }
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                Console.WriteLine("Database connection disposed.");
            }
        }
    }
}
