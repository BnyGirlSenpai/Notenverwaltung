//using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace App.App.services
{
    internal class StudentDatabaseSyncronisationService()
    {
        private readonly string _mysqlConnStr = Path.Combine(Directory.GetCurrentDirectory(), "OnlineNotenverwaltung.db3");
        private readonly string _sqliteConnStr = Path.Combine(Directory.GetCurrentDirectory(), "OfflineNotenverwaltung.db3");

        public async Task SyncStudentMarksFromMySqlToSqliteAsync()
        {
            using var mysqlConn = new SQLiteConnection($"Data Source={_mysqlConnStr};Version=3;");
            using var sqliteConn = new SQLiteConnection($"Data Source={_sqliteConnStr};Version=3;");

            await mysqlConn.OpenAsync();
            await sqliteConn.OpenAsync();

            string mysqlQuery = "SELECT student_id, lesson_id, student_mark FROM marks";
            using var mysqlCmd = new SQLiteCommand(mysqlQuery, mysqlConn);
            using var mysqlReader = await mysqlCmd.ExecuteReaderAsync();

            while (await mysqlReader.ReadAsync())
            {
                string sqliteQuery = @"
                    UPDATE marks 
                    SET student_mark = @studentMark 
                    WHERE student_id = @studentId 
                    AND lesson_id = @lessonId";

                using var sqliteCmd = new SQLiteCommand(sqliteQuery, sqliteConn);
                sqliteCmd.Parameters.AddWithValue("@studentId", mysqlReader["student_id"]);
                sqliteCmd.Parameters.AddWithValue("@lessonId", mysqlReader["lesson_id"]);
                sqliteCmd.Parameters.AddWithValue("@studentMark", mysqlReader["student_mark"]);

                await sqliteCmd.ExecuteNonQueryAsync();
            }
        }

        public async Task SyncStudentMarksFromSqliteToMySqlAsync()
        {
            using var mysqlConn = new SQLiteConnection($"Data Source={_mysqlConnStr};Version=3;");
            using var sqliteConn = new SQLiteConnection($"Data Source={_sqliteConnStr};Version=3;");

            await mysqlConn.OpenAsync();
            await sqliteConn.OpenAsync();

            string sqliteQuery = "SELECT student_id, lesson_id, student_mark FROM marks";
            using var sqliteCmd = new SQLiteCommand(sqliteQuery, sqliteConn);
            using var sqliteReader = await sqliteCmd.ExecuteReaderAsync();

            while (await sqliteReader.ReadAsync())
            {
                string mysqlQuery = @"
                    UPDATE marks 
                    SET student_mark = @studentMark 
                    WHERE student_id = @studentId 
                    AND lesson_id = @lessonId";

                using var mysqlCmd = new SQLiteCommand(mysqlQuery, mysqlConn);
                mysqlCmd.Parameters.AddWithValue("@studentId", sqliteReader["student_id"]);
                mysqlCmd.Parameters.AddWithValue("@lessonId", sqliteReader["lesson_id"]);
                mysqlCmd.Parameters.AddWithValue("@studentMark", sqliteReader["student_mark"]);

                await mysqlCmd.ExecuteNonQueryAsync();
            }
        }
    }
}