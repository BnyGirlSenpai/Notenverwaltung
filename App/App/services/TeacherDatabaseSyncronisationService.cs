using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace App.App.services
{
    internal class TeacherDatabaseSyncronisationService
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

            string sqliteQuery = $"SELECT * FROM {tableName}";
            using var sqliteCmd = new SQLiteCommand(sqliteQuery, sqliteConn);
            using var sqliteReader = await sqliteCmd.ExecuteReaderAsync();

            var columnNames = GetColumnNames((SQLiteDataReader)sqliteReader);

            if (tableName == "marks")
            {
                columnNames = columnNames.Where(col => col != "student_mark").ToArray();
            }

            while (await sqliteReader.ReadAsync())
            {
                var insertOrUpdateQuery = BuildInsertOrUpdateQuery(tableName, columnNames);

                using var mysqlCmd = new MySqlCommand(insertOrUpdateQuery, mysqlConn);

                for (int i = 0; i < columnNames.Length; i++)
                {
                    var value = sqliteReader.IsDBNull(i) ? null : sqliteReader.GetValue(i);
                    mysqlCmd.Parameters.AddWithValue($"@{columnNames[i]}", value);
                }

                await mysqlCmd.ExecuteNonQueryAsync();
                mysqlCmd.Parameters.Clear();
            }

            await SyncStudentMarksAsync(sqliteConn, mysqlConn);

            Console.WriteLine($"Table {tableName} synchronized successfully.");
        }

        private static async Task SyncStudentMarksAsync(SQLiteConnection sqliteConn, MySqlConnection mysqlConn)
        {
            Console.WriteLine("Synchronizing all student_mark fields from SQLite to MySQL");

            string sqliteQuery = "SELECT mark_id, student_mark FROM marks";
            using var sqliteCmd = new SQLiteCommand(sqliteQuery, sqliteConn);
            using var sqliteReader = await sqliteCmd.ExecuteReaderAsync();

            using var mysqlCmd = new MySqlCommand();

            
            mysqlCmd.Connection = mysqlConn;

            while (await sqliteReader.ReadAsync())
            {
                var mark_id = sqliteReader.GetInt32(0);
                var studentMark = sqliteReader.IsDBNull(1) ? null : sqliteReader.GetValue(1); 

                string mysqlUpdateQuery = "UPDATE marks SET student_mark = @studentMark WHERE mark_id = @mark_id";
                mysqlCmd.CommandText = mysqlUpdateQuery;
                mysqlCmd.Parameters.AddWithValue("@studentMark", studentMark ?? DBNull.Value);
                mysqlCmd.Parameters.AddWithValue("@mark_id", mark_id);

                await mysqlCmd.ExecuteNonQueryAsync();
                mysqlCmd.Parameters.Clear();
            }

            Console.WriteLine("All student_mark fields have been synchronized from SQLite to MySQL successfully.");
        }

        private static string[] GetColumnNames(SQLiteDataReader reader)
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
            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values}) ON DUPLICATE KEY UPDATE ";

            var updateParts = columnNames.Select(col => $"{col} = @{col}");
            query += string.Join(", ", updateParts);

            return query;
        }
    }
}
