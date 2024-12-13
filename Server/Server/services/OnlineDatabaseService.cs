using System.Data.SQLite;

namespace HttpServer.Server.services
{
    internal class OnlineDatabaseService
    {
        public static void InitializeOnlineDatabase(string onlineDatabasePath)
        {
            try
            {
                if (!File.Exists(onlineDatabasePath))
                {
                    SQLiteConnection.CreateFile(onlineDatabasePath);
                }

                using var connection = new SQLiteConnection($"Data Source={onlineDatabasePath};Version=3;");
                connection.Open();

                using var transaction = connection.BeginTransaction();

                string createRolesTable = @"
                    CREATE TABLE IF NOT EXISTS roles (
                        role_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        role_name TEXT NOT NULL
                    );";

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

                string createCoursesTable = @"
                    CREATE TABLE IF NOT EXISTS courses (
                        course_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        course_name TEXT NOT NULL,
                        course_code TEXT NOT NULL,
                        teacher_id INTEGER NULL,
                        FOREIGN KEY (teacher_id) REFERENCES users (user_id) ON UPDATE NO ACTION ON DELETE NO ACTION
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

                string insertSampleData = @"
                    -- Ensure the roles table has entries
                    INSERT INTO roles (role_name)
                    SELECT 'Teacher'
                    WHERE NOT EXISTS (SELECT 1 FROM roles WHERE role_name = 'Teacher');

                    INSERT INTO roles (role_name)
                    SELECT 'Student'
                    WHERE NOT EXISTS (SELECT 1 FROM roles WHERE role_name = 'Student');

                    -- Insert users into the users table
                    INSERT INTO users (first_name, last_name, date_of_birth, email, role_id, password)
                    SELECT 'John', 'Doe', '1980-05-15', 'johndoe@example.com', 1, 'securepassword123'
                    WHERE NOT EXISTS (SELECT 1 FROM users WHERE email = 'johndoe@example.com');

                    INSERT INTO users (first_name, last_name, date_of_birth, email, role_id, password)
                    SELECT 'Jane', 'Smith', '1999-08-22', 'janesmith@example.com', 2, 'password456'
                    WHERE NOT EXISTS (SELECT 1 FROM users WHERE email = 'janesmith@example.com');

                    -- Insert courses into the courses table
                    INSERT INTO courses (course_name, course_code, teacher_id)
                    SELECT 'Introduction to Database Systems', 'CS101', (SELECT user_id FROM users WHERE email = 'johndoe@example.com')
                    WHERE NOT EXISTS (SELECT 1 FROM courses WHERE course_code = 'CS101');

                    INSERT INTO courses (course_name, course_code, teacher_id)
                    SELECT 'Advanced SQL', 'CS201', (SELECT user_id FROM users WHERE email = 'johndoe@example.com')
                    WHERE NOT EXISTS (SELECT 1 FROM courses WHERE course_code = 'CS201');

                    -- Insert sample educational branches
                    INSERT INTO educational_branches (notenskala, bezeichnung)
                    SELECT '0-100%', 'IHK'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM educational_branches 
                        WHERE bezeichnung = 'IHK'
                    );

                    -- Insert enrollments into the enrollments table
                    INSERT INTO enrollments (student_id, course_id, enrollment_date, educational_branch_id)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT course_id FROM courses WHERE course_code = 'CS101'),
                        '2024-01-10',
                        1
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM enrollments 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND course_id = (SELECT course_id FROM courses WHERE course_code = 'CS101')
                    );

                    INSERT INTO enrollments (student_id, course_id, enrollment_date, educational_branch_id)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT course_id FROM courses WHERE course_code = 'CS201'),
                        '2024-01-15',
                        1
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM enrollments 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND course_id = (SELECT course_id FROM courses WHERE course_code = 'CS201')
                    );

                    -- Insert lessons into the lessons table
                    INSERT INTO lessons (course_id, lesson_name, lesson_date, lesson_type)
                    SELECT 
                        (SELECT course_id FROM courses WHERE course_code = 'CS101'),
                        'Introduction to Relational Databases',
                        '2024-01-10',
                        'Lecture'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM lessons 
                        WHERE course_id = (SELECT course_id FROM courses WHERE course_code = 'CS101') 
                            AND lesson_name = 'Introduction to Relational Databases'
                    );

                    INSERT INTO lessons (course_id, lesson_name, lesson_date, lesson_type)
                    SELECT 
                        (SELECT course_id FROM courses WHERE course_code = 'CS101'),
                        'ER Modeling',
                        '2024-01-12',
                        'Workshop'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM lessons 
                        WHERE course_id = (SELECT course_id FROM courses WHERE course_code = 'CS101') 
                            AND lesson_name = 'ER Modeling'
                    );

                    INSERT INTO lessons (course_id, lesson_name, lesson_date, lesson_type)
                    SELECT 
                        (SELECT course_id FROM courses WHERE course_code = 'CS201'),
                        'Advanced Joins and Optimization',
                        '2024-02-01',
                        'Lecture'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM lessons 
                        WHERE course_id = (SELECT course_id FROM courses WHERE course_code = 'CS201') 
                            AND lesson_name = 'Advanced Joins and Optimization'
                    );

                    INSERT INTO lessons (course_id, lesson_name, lesson_date, lesson_type)
                    SELECT 
                        (SELECT course_id FROM courses WHERE course_code = 'CS201'),
                        'Stored Procedures',
                        '2024-02-05',
                        'Workshop'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM lessons 
                        WHERE course_id = (SELECT course_id FROM courses WHERE course_code = 'CS201') 
                            AND lesson_name = 'Stored Procedures'
                    );

                    -- Insert attendance into the attendance table
                    INSERT INTO attendance (student_id, lesson_id, attendance_date, status)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT lesson_id FROM lessons WHERE lesson_name = 'Introduction to Relational Databases'),
                        '2024-01-10',
                        'Present'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM attendance 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND lesson_id = (SELECT lesson_id FROM lessons WHERE lesson_name = 'Introduction to Relational Databases')
                    );

                    INSERT INTO attendance (student_id, lesson_id, attendance_date, status)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT lesson_id FROM lessons WHERE lesson_name = 'ER Modeling'),
                        '2024-01-12',
                        'Absent'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM attendance 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND lesson_id = (SELECT lesson_id FROM lessons WHERE lesson_name = 'ER Modeling')
                    );

                    INSERT INTO attendance (student_id, lesson_id, attendance_date, status)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT lesson_id FROM lessons WHERE lesson_name = 'Advanced Joins and Optimization'),
                        '2024-02-01',
                        'Present'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM attendance 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND lesson_id = (SELECT lesson_id FROM lessons WHERE lesson_name = 'Advanced Joins and Optimization')
                    );

                    INSERT INTO attendance (student_id, lesson_id, attendance_date, status)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT lesson_id FROM lessons WHERE lesson_name = 'Stored Procedures'),
                        '2024-02-01',
                        'Excused'
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM attendance 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND lesson_id = (SELECT lesson_id FROM lessons WHERE lesson_name = 'Stored Procedures')
                    );

                    -- Insert marks into the marks table
                    INSERT INTO marks (student_id, lesson_id, teacher_mark, student_mark, mark_date, teacher_id, final_mark)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT lesson_id FROM lessons WHERE lesson_name = 'Introduction to Relational Databases'),
                        85, -- Teacher's mark
                        80, -- Student's mark
                        '2024-01-15', -- Mark date
                        (SELECT user_id FROM users WHERE email = 'johndoe@example.com'), -- Teacher ID
                        83 -- Final mark
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM marks 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND lesson_id = (SELECT lesson_id FROM lessons WHERE lesson_name = 'Introduction to Relational Databases')
                    );

                    INSERT INTO marks (student_id, lesson_id, teacher_mark, student_mark, mark_date, teacher_id, final_mark)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT lesson_id FROM lessons WHERE lesson_name = 'ER Modeling'),
                        90, -- Teacher's mark
                        88, -- Student's mark
                        '2024-01-20', -- Mark date
                        (SELECT user_id FROM users WHERE email = 'johndoe@example.com'), -- Teacher ID
                        89 -- Final mark
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM marks 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND lesson_id = (SELECT lesson_id FROM lessons WHERE lesson_name = 'ER Modeling')
                    );

                    INSERT INTO marks (student_id, lesson_id, teacher_mark, student_mark, mark_date, teacher_id, final_mark)
                    SELECT 
                        (SELECT user_id FROM users WHERE email = 'janesmith@example.com'),
                        (SELECT lesson_id FROM lessons WHERE lesson_name = 'Advanced Joins and Optimization'),
                        45, -- Teacher's mark
                        60, -- Student's mark
                        '2024-02-01', -- Mark date
                        (SELECT user_id FROM users WHERE email = 'johndoe@example.com'), -- Teacher ID
                        50 -- Final mark
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM marks 
                        WHERE student_id = (SELECT user_id FROM users WHERE email = 'janesmith@example.com') 
                            AND lesson_id = (SELECT lesson_id FROM lessons WHERE lesson_name = 'Advanced Joins and Optimization')
                    );
                ";

                ExecuteQuery(createRolesTable, connection);
                ExecuteQuery(createUsersTable, connection);
                ExecuteQuery(createCoursesTable, connection);
                ExecuteQuery(createLessonsTable, connection);
                ExecuteQuery(createEducationalBranchesTable, connection);
                ExecuteQuery(createEnrollmentsTable, connection);
                ExecuteQuery(createMarksTable, connection);
                ExecuteQuery(createAttendanceTable, connection);
                ExecuteQuery(createIndexes, connection);
                ExecuteQuery(insertSampleData, connection);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing the database: {ex}");
            }
        }
        private static void ExecuteQuery(string query, SQLiteConnection connection)
        {
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
        }
    }
}
