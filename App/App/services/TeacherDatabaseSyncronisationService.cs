﻿//using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace App.App.services
{
    internal class TeacherDatabaseSynchronisationService() 
    {
        private readonly string _mysqlConnStr = Path.Combine(Directory.GetCurrentDirectory(), "OnlineNotenverwaltung.db3");
        private readonly string _sqliteConnStr = Path.Combine(Directory.GetCurrentDirectory(), "OfflineNotenverwaltung.db3");

        // FROM SQLITE TO MYSQL
        public async Task SyncAllTablesFromSqliteToMySqlAsync()
        {
            string[] tables = { "marks", "attendance" };
            using var mysqlConn = new SQLiteConnection($"Data Source={_mysqlConnStr};Version=3;");
            using var sqliteConn = new SQLiteConnection($"Data Source={_sqliteConnStr};Version=3;");

            await mysqlConn.OpenAsync();
            await sqliteConn.OpenAsync();

            foreach (var table in tables)
            {
                await SyncFromSqliteToMySqlAsync(mysqlConn, sqliteConn, table);
            }
        }

        private static async Task SyncFromSqliteToMySqlAsync(SQLiteConnection mysqlConn, SQLiteConnection sqliteConn, string tableName)
        {
            string sqliteQuery = $"SELECT * FROM {tableName}";
            using var sqliteCmd = new SQLiteCommand(sqliteQuery, sqliteConn);
            using var sqliteReader = await sqliteCmd.ExecuteReaderAsync();

            var columnNames = GetSqlLiteColumnNames((SQLiteDataReader)sqliteReader);

            if (tableName == "marks")
            {
                columnNames = columnNames.Where(col => col != "student_mark" + "mark_id" + "student_id" + "lesson_id" + "teacher_id").ToArray();
            }

            if (tableName == "attendance")
            {
                columnNames = columnNames.Where(col => col != "attendance_id" + "student_id" + "lesson_id" + "attendance_date").ToArray();
            }

            while (await sqliteReader.ReadAsync())
            {
                var insertOrUpdateQuery = InsertOrUpdateQueryForSqlite(tableName, columnNames);

                using var mysqlCmd = new SQLiteCommand(insertOrUpdateQuery, mysqlConn);

                for (int i = 0; i < columnNames.Length; i++)
                {
                    var value = sqliteReader.IsDBNull(i) ? null : sqliteReader.GetValue(i);
                    mysqlCmd.Parameters.AddWithValue($"@{columnNames[i]}", value);
                }

                await mysqlCmd.ExecuteNonQueryAsync();
                mysqlCmd.Parameters.Clear();
            }

            await SyncStudentMarksAsync(sqliteConn, mysqlConn);
        }

        private static async Task SyncStudentMarksAsync(SQLiteConnection sqliteConn, SQLiteConnection mysqlConn)
        {
            string sqliteQuery = "SELECT mark_id, student_mark FROM marks";
            using var sqliteCmd = new SQLiteCommand(sqliteQuery, sqliteConn);
            using var sqliteReader = await sqliteCmd.ExecuteReaderAsync();

            using var mysqlCmd = new SQLiteCommand();

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
        }

        /*
        private static string InsertOrUpdateQueryForMySQL(string tableName, string[] columnNames)
        {
            string columns = string.Join(", ", columnNames);
            string values = string.Join(", ", columnNames.Select(col => $"@{col}"));
            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values}) ON DUPLICATE KEY UPDATE ";

            var updateParts = columnNames.Select(col => $"{col} = @{col}");
            query += string.Join(", ", updateParts);

            return query;
        }*/


        //FROM MYSQL TO SQLITE
        public async Task SyncAllTablesFromMySqlToSqliteAsync()
        {
            string[] tables = { "users", "roles", "marks", "lessons", "enrollments", "courses", "attendance" };

            using var mysqlConn = new SQLiteConnection($"Data Source={_mysqlConnStr};Version=3;");
            using var sqliteConn = new SQLiteConnection($"Data Source={_sqliteConnStr};Version=3;");

            await mysqlConn.OpenAsync();
            await sqliteConn.OpenAsync();

            foreach (var table in tables)
            {
                await SyncTableFromMySqlToSqliteAsync(mysqlConn, sqliteConn, table);
            }
        }

        private static async Task SyncTableFromMySqlToSqliteAsync(SQLiteConnection mysqlConn, SQLiteConnection sqliteConn, string tableName)
        {
            string mysqlQuery = $"SELECT * FROM {tableName}";
            using var mysqlCmd = new SQLiteCommand(mysqlQuery, mysqlConn);
            using var mysqlReader = await mysqlCmd.ExecuteReaderAsync();

            var columnNames = GetMySqlColumnNames((SQLiteDataReader)mysqlReader);

            while (await mysqlReader.ReadAsync())
            {
                string sqliteQuery = InsertOrUpdateQueryForSqlite(tableName, columnNames);
                using var sqliteCmd = new SQLiteCommand(sqliteQuery, sqliteConn);

                for (int i = 0; i < columnNames.Length; i++)
                {
                    var value = mysqlReader.IsDBNull(i) ? null : mysqlReader.GetValue(i);
                    sqliteCmd.Parameters.AddWithValue($"@{columnNames[i]}", value);
                }

                await sqliteCmd.ExecuteNonQueryAsync();
            }
        }

        private static string InsertOrUpdateQueryForSqlite(string tableName, string[] columnNames)
        {
            string columns = string.Join(", ", columnNames);
            string values = string.Join(", ", columnNames.Select(col => $"@{col}"));
            string query = $"INSERT OR REPLACE INTO {tableName} ({columns}) VALUES ({values})";

            return query;
        }

        private static string[] GetSqlLiteColumnNames(SQLiteDataReader reader)
        {
            int fieldCount = reader.FieldCount;
            string[] columnNames = new string[fieldCount];

            for (int i = 0; i < fieldCount; i++)
            {
                columnNames[i] = reader.GetName(i);
            }

            return columnNames;
        }

        private static string[] GetMySqlColumnNames(SQLiteDataReader reader)
        {
            int fieldCount = reader.FieldCount;
            string[] columnNames = new string[fieldCount];

            for (int i = 0; i < fieldCount; i++)
            {
                columnNames[i] = reader.GetName(i);
            }

            return columnNames;
        }
    }
}