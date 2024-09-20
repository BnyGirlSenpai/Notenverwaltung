using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace App.App.services
{
    internal class StudentDatabaseSyncronisationService
    {
        private string _mysqlConnStr = "Server=localhost;Database=notenverwaltung;User ID=root;Password=password;";
        private string _sqliteConnStr = "C:\\Users\\drebes\\Berufsschule\\SDM\\SQL\\Database\\Notenverwaltung.db3";

        public async Task SyncAllTablesAsync()
        {
            string[] tables = { "users", "roles", "marks", "lessons", "enrollments", "courses", "attendance" };

            using var mysqlConn = new MySqlConnection(_mysqlConnStr);
            using var sqliteConn = new SQLiteConnection($"Data Source={_sqliteConnStr};Version=3;");
            await mysqlConn.OpenAsync();
            await sqliteConn.OpenAsync();

            foreach (var table in tables)
            {
                await SyncTableAsync(mysqlConn, sqliteConn, table);
            }

            Console.WriteLine("Synchronization of all tables complete.");
        }

        private static async Task SyncTableAsync(MySqlConnection mysqlConn, SQLiteConnection sqliteConn, string tableName)
        {
            Console.WriteLine($"Synchronizing table: {tableName}");

            string mysqlQuery = $"SELECT * FROM {tableName}";
            using var mysqlCmd = new MySqlCommand(mysqlQuery, mysqlConn);
            using var mysqlReader = await mysqlCmd.ExecuteReaderAsync();

            var columnNames = GetColumnNames((MySqlDataReader)mysqlReader);

            using var sqliteCmd = new SQLiteCommand(sqliteConn);
            while (await mysqlReader.ReadAsync())
            {
                var insertOrUpdateQuery = BuildInsertOrUpdateQuery(tableName, columnNames);
                sqliteCmd.CommandText = insertOrUpdateQuery;

                for (int i = 0; i < columnNames.Length; i++)
                {
                    sqliteCmd.Parameters.AddWithValue($"@{columnNames[i]}", mysqlReader.GetValue(i));
                }

                await sqliteCmd.ExecuteNonQueryAsync();
                sqliteCmd.Parameters.Clear();
            }

            Console.WriteLine($"Table {tableName} synchronized successfully.");
        }

        private static string[] GetColumnNames(MySqlDataReader reader)
        {
            int fieldCount = reader.FieldCount;
            string[] columnNames = new string[fieldCount];

            for (int i = 0; i < fieldCount; i++)
            {
                columnNames[i] = reader.GetName(i);
            }

            return columnNames;
        }

        private static string BuildInsertOrUpdateQuery(string tableName, string[] columnNames)
        {
            string columns = string.Join(", ", columnNames);
            string values = string.Join(", ", columnNames.Select(col => $"@{col}"));

            string query = $"INSERT OR REPLACE INTO {tableName} ({columns}) VALUES ({values})";
            return query;
        }
    }
}
