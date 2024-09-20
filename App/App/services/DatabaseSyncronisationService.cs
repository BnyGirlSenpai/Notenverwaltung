using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace App.App.services
{
    internal class DatabaseSyncronisationService
    {
        private string _mysqlConnStr = "Server=localhost;Database=notenverwaltung;User ID=root;Password=password;";
        private string _sqliteConnStr = "C:\\Users\\drebes\\Berufsschule\\SDM\\SQL\\Database\\Notenverwaltung.db3"; 
         
        public void SyncAllTables()
        {
            string[] tables = { "users", "roles", "marks", "lessons", "enrollments", "courses", "attendance" };

            using var mysqlConn = new MySqlConnection(_mysqlConnStr);
            using var sqliteConn = new SQLiteConnection($"Data Source={_sqliteConnStr};Version=3;");
            mysqlConn.Open();
            sqliteConn.Open();

            foreach (var table in tables)
            {
                SyncTable(mysqlConn, sqliteConn, table);
            }

            Console.WriteLine("Synchronization of all tables complete.");
        }

        private static void SyncTable(MySqlConnection mysqlConn, SQLiteConnection sqliteConn, string tableName)
        {
            Console.WriteLine($"Synchronizing table: {tableName}");

            string mysqlQuery = $"SELECT * FROM {tableName}";
            using (var mysqlCmd = new MySqlCommand(mysqlQuery, mysqlConn))
            using (var mysqlReader = mysqlCmd.ExecuteReader())
            {
                var columnNames = GetColumnNames(mysqlReader);

                using var sqliteCmd = new SQLiteCommand(sqliteConn);
                while (mysqlReader.Read())
                {
                    var insertOrUpdateQuery = BuildInsertOrUpdateQuery(tableName, columnNames);
                    sqliteCmd.CommandText = insertOrUpdateQuery;

                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        sqliteCmd.Parameters.AddWithValue($"@{columnNames[i]}", mysqlReader.GetValue(i));
                    }

                    sqliteCmd.ExecuteNonQuery();
                    sqliteCmd.Parameters.Clear();
                }
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


