using System.Data.SQLite;

namespace App.App.controller
{
    internal class LocalBaseController
    {
        private static readonly string _connectionString = "Data Source=C:\\Users\\drebes\\Berufsschule\\SDM\\MyProjects\\Notenverwaltung\\Database\\Notenverwaltung.db3;Version=3;";

        protected static async Task<SQLiteConnection> GetOpenConnectionAsync()
        {
            var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        protected static async Task<List<T>> ExecuteQueryAsync<T>(SQLiteCommand command, Func<SQLiteDataReader, Task<T>> readFunc)
        {
            var results = new List<T>();

            try
            {
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var result = await readFunc((SQLiteDataReader)reader);
                    results.Add(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
            }

            return results;
        }
    }
}
