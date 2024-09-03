using System;
using System.Data;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace NotenverwaltungsApp
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

        public Database(DatabaseType dbType)
        {
            _dbType = dbType;

            switch (_dbType)
            {
                case DatabaseType.SQLite:
                    _connectionString = "Data Source=C:\\Users\\drebes\\Berufsschule\\SDM\\SQL\\sqlitespy_1.9.10\\Notenverwaltung.db3";
                    break;

                case DatabaseType.MySQL:
                    _connectionString = BuildMySqlConnectionString();
                    break;

                default:
                    throw new NotSupportedException("Database type not supported.");
            }
        }

        private static string BuildMySqlConnectionString()
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST");
            string user = Environment.GetEnvironmentVariable("DB_USER");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            string database = Environment.GetEnvironmentVariable("DB_NAME");

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
                switch (_dbType)
                {
                    case DatabaseType.SQLite:
                        _connection = new SQLiteConnection(_connectionString);
                        break;

                    case DatabaseType.MySQL:
                        _connection = new MySqlConnection(_connectionString);
                        break;

                    default:
                        throw new NotSupportedException("Database type not supported.");
                }

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
