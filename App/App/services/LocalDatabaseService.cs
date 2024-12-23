﻿using System.Data.SQLite;

namespace App.App.services
{
    internal class LocalDatabaseService()
    {
        public static async Task<string> IsServerConnectedAsync()
        {
             string localDatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "OfflineNotenverwaltung.db3");

            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync("http://localhost:5000/ping");

                if (response.IsSuccessStatusCode)
                {
                    InitializeLocalDatabase(localDatabasePath);
                    return "Online";
                }
                else
                {
                    Console.WriteLine($"Server responded with status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Server is unreachable. Falling back to offline mode.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            InitializeLocalDatabase(localDatabasePath);
            return "Offline";
        }

        public static void InitializeLocalDatabase(string _sqliteConnStr)
        {
            try
            {
                if (!File.Exists(_sqliteConnStr))
                {
                    SQLiteConnection.CreateFile(_sqliteConnStr);
                }

                using var connection = new SQLiteConnection($"Data Source={_sqliteConnStr};Version=3;");
                connection.Open();

                using var transaction = connection.BeginTransaction();
                string createUsersTable = @"
                        CREATE TABLE IF NOT EXISTS users (
                            user_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            first_name TEXT NOT NULL,
                            last_name TEXT NOT NULL,
                            date_of_birth DATE NULL,
                            email TEXT NULL,
                            role_id INTEGER NULL,
                            password TEXT NOT NULL,
                            FOREIGN KEY (role_id) REFERENCES roles (role_id) ON UPDATE NO ACTION ON DELETE NO ACTION
                        );";

                string createRolesTable = @"
                        CREATE TABLE IF NOT EXISTS roles (
                            role_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            role_name TEXT NOT NULL
                        );";

                string createMarksTable = @"
                        CREATE TABLE IF NOT EXISTS marks (
                            mark_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            student_id INTEGER NULL,
                            lesson_id INTEGER NULL,
                            teacher_mark INTEGER NOT NULL,
                            student_mark INTEGER NOT NULL,
                            mark_date DATE NULL,
                            teacher_id INTEGER NULL,
                            final_mark INTEGER NULL,
                            FOREIGN KEY (lesson_id) REFERENCES lessons (lesson_id) ON UPDATE NO ACTION ON DELETE NO ACTION,
                            FOREIGN KEY (teacher_id) REFERENCES users (user_id) ON UPDATE NO ACTION ON DELETE NO ACTION,
                            FOREIGN KEY (student_id) REFERENCES users (user_id) ON UPDATE NO ACTION ON DELETE NO ACTION
                        );";

                string createLessonsTable = @"
                        CREATE TABLE IF NOT EXISTS lessons (
                            lesson_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            course_id INTEGER NULL,
                            lesson_name TEXT NOT NULL,
                            lesson_date DATE NULL,
                            lesson_type TEXT NULL,
                            FOREIGN KEY (course_id) REFERENCES courses (course_id) ON UPDATE NO ACTION ON DELETE NO ACTION
                        );";

                string createCoursesTable = @"
                        CREATE TABLE IF NOT EXISTS courses (
                            course_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            course_name TEXT NOT NULL,
                            course_code TEXT NOT NULL,
                            teacher_id INTEGER NULL,
                            FOREIGN KEY (teacher_id) REFERENCES users (user_id) ON UPDATE NO ACTION ON DELETE NO ACTION
                        );";

                string createEducationalBranchesTable = @"
                        CREATE TABLE IF NOT EXISTS educational_branches (
                            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            notenskala TEXT NULL DEFAULT NULL,
                            bezeichnung TEXT NOT NULL
                        );";

                string createEnrollmentsTable = @"
                        CREATE TABLE IF NOT EXISTS enrollments (
                            enrollment_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            student_id INTEGER NULL DEFAULT NULL,
                            course_id INTEGER NOT NULL,
                            enrollment_date DATE NULL DEFAULT NULL,
                            educational_branch_id INTEGER NOT NULL,
                            FOREIGN KEY (course_id) REFERENCES courses (course_id) ON UPDATE NO ACTION ON DELETE NO ACTION,
                            FOREIGN KEY (student_id) REFERENCES users (user_id) ON UPDATE NO ACTION ON DELETE NO ACTION,
                            FOREIGN KEY (educational_branch_id) REFERENCES educational_branches (id) ON UPDATE NO ACTION ON DELETE NO ACTION
                        );";
                
                string createAttendanceTable = @"
                        CREATE TABLE IF NOT EXISTS attendance (
                            attendance_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            student_id INTEGER NULL,
                            lesson_id INTEGER NULL,
                            attendance_date DATE NULL,
                            status TEXT NULL,
                            FOREIGN KEY (lesson_id) REFERENCES lessons (lesson_id) ON UPDATE NO ACTION ON DELETE NO ACTION,
                            FOREIGN KEY (student_id) REFERENCES users (user_id) ON UPDATE NO ACTION ON DELETE NO ACTION
                        );";

                string createIndexes = @"
                    CREATE INDEX IF NOT EXISTS idx_attendance_lesson ON attendance (lesson_id);
                    CREATE INDEX IF NOT EXISTS idx_attendance_student ON attendance (student_id);
                    CREATE INDEX IF NOT EXISTS idx_courses_teacher ON courses (teacher_id);
                    CREATE UNIQUE INDEX IF NOT EXISTS unique_course_code ON courses (course_code);
                    CREATE INDEX IF NOT EXISTS idx_enrollments_course ON enrollments (course_id);
                    CREATE INDEX IF NOT EXISTS idx_enrollments_student ON enrollments (student_id);
                    CREATE INDEX IF NOT EXISTS idx_lessons_course ON lessons (course_id);
                    CREATE INDEX IF NOT EXISTS idx_marks_teacher ON marks (teacher_id);
                    CREATE INDEX IF NOT EXISTS idx_marks_lesson ON marks (lesson_id);
                    CREATE INDEX IF NOT EXISTS idx_marks_student ON marks (student_id);
                    CREATE INDEX IF NOT EXISTS idx_users_role ON users (role_id);
                    CREATE INDEX IF NOT EXISTS idx_users_email ON users (email);
                    CREATE UNIQUE INDEX IF NOT EXISTS unique_user_email ON users (email);
                ";

                ExecuteQuery(createUsersTable, connection);
                ExecuteQuery(createRolesTable, connection);
                ExecuteQuery(createMarksTable, connection);
                ExecuteQuery(createLessonsTable, connection);
                ExecuteQuery(createCoursesTable, connection);
                ExecuteQuery(createEducationalBranchesTable, connection);
                ExecuteQuery(createEnrollmentsTable, connection);
                ExecuteQuery(createAttendanceTable, connection);
                ExecuteQuery(createIndexes, connection);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing the database: {ex.Message}");
            }
        }

        private static void ExecuteQuery(string query, SQLiteConnection connection)
        {
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery(); 
        }
    }
}
