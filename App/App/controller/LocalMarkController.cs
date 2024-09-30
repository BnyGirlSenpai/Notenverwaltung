using App.App.repositorys;
using System.Data.SQLite;

namespace App.App.controller
{
    internal class LocalMarkController : LocalBaseController
    {
        public static async Task<List<MarkRepository>> GetMarksForLessonsAsync(string studentId, string lessonId)
        {
            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand(@"SELECT m.mark_id, 
                                              m.student_mark, 
                                              m.teacher_mark, 
                                              m.final_mark, 
                                              u.last_name, 
                                              u.first_name 
                                              FROM marks m 
                                              LEFT JOIN users u ON m.teacher_id = u.user_id 
                                              WHERE m.student_id = @studentId 
                                              AND m.lesson_id = @lessonId;", connection);

            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@lessonId", lessonId);

            var marks = await ExecuteQueryAsync(command, async reader => new MarkRepository
            {
                MarkId = reader["mark_id"]?.ToString() ?? "Unknown",
                StudentMark = reader["student_mark"]?.ToString() ?? "Unknown",
                TeacherMark = reader["teacher_mark"]?.ToString() ?? "Unknown",
                FinalMark = reader["final_mark"]?.ToString() ?? "Unknown",
            });

            if (marks.Count == 0)
            {
                marks.Add(new MarkRepository
                {
                    MarkId = "N.a.N",
                    StudentMark = "N.a.N",
                    TeacherMark = "N.a.N",
                    FinalMark = "N.a.N"
                });
            }

            return marks;
        }

        public static async Task<string> UpdateMarksForLessonAsync(string studentId, string lessonId, string teacherId, string teacherMark, string finalMark)
        {
            string message = "Update successful";
            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand("UPDATE marks SET teacher_mark = @teacherMark, final_mark = @finalMark, teacher_id = @teacherId WHERE student_id = @studentId AND lesson_id = @lessonId", connection);

            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@lessonId", lessonId);
            command.Parameters.AddWithValue("@teacherId", teacherId);
            command.Parameters.AddWithValue("@teacherMark", teacherMark);
            command.Parameters.AddWithValue("@finalMark", finalMark);

            try
            {
                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    message = "No record found to update";
                }

                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Mark: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public static async Task<string> UpdateStudentMarkForLessonAsync(string studentId, string lessonId, string studentMark)
        {
            string message = "Update successful";

            using var connection = await GetOpenConnectionAsync();
            var command = new SQLiteCommand("UPDATE marks SET student_Mark = @studentMark WHERE student_id = @studentId AND lesson_id = @lessonId", connection);

            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@lessonId", lessonId);
            command.Parameters.AddWithValue("@studentMark", studentMark);

            try
            {
                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    message = "No record found to update";
                }

                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Mark: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}
