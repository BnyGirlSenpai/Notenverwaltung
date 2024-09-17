﻿using System.Text.Json.Serialization;
using static NotenverwaltungsApp.Database;

namespace NotenverwaltungsApp.Server.controllers
{
    internal class MarkController
    {
        public class Mark
        {
            [JsonPropertyName("markId")]
            public string MarkId { get; set; }

            [JsonPropertyName("studentMark")]
            public string StudentMark { get; set; }

            [JsonPropertyName("teacherMark")]
            public string TeacherMark { get; set; }

            [JsonPropertyName("finalMark")]
            public string FinalMark { get; set; }

            [JsonPropertyName("teacherFirstname")]
            public string TeacherFirstname { get; set; }

            [JsonPropertyName("teacherLastname")]
            public string TeacherLastname { get; set; }
        }

        public static List<Mark> GetMarksForLessons(string studentId, string lessonId)
        {
            var marks = new List<Mark>();

            using var db = new Database(DatabaseType.SQLite);
            {
                try
                {
                    db.Connect_to_Database();
                    var connection = db.GetConnection();

                    string query = @"
                        SELECT m.mark_id, 
                               m.student_mark, 
                               m.teacher_mark, 
                               m.final_mark, 
                               u.last_name, 
                               u.first_name
                        FROM marks m
                        LEFT JOIN users u ON m.teacher_id = u.user_id
                        WHERE m.student_id = @studentId
                          AND m.lesson_id = @lessonId;
                    ";

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    var studentParameter = command.CreateParameter();
                    studentParameter.ParameterName = "@studentId";
                    studentParameter.Value = studentId;
                    command.Parameters.Add(studentParameter);

                    var lessonParameter = command.CreateParameter();
                    lessonParameter.ParameterName = "@lessonId";
                    lessonParameter.Value = lessonId;
                    command.Parameters.Add(lessonParameter);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var mark = new Mark
                        {
                            MarkId = reader["mark_id"]?.ToString() ?? "Unknown",
                            StudentMark = reader["student_mark"]?.ToString() ?? "Unknown",
                            TeacherMark = reader["teacher_mark"]?.ToString() ?? "Unknown",
                            FinalMark = reader["final_mark"]?.ToString() ?? "Unknown",
                            TeacherFirstname = reader["first_name"]?.ToString() ?? "Unknown",
                            TeacherLastname = reader["last_name"]?.ToString() ?? "Unknown",
                        };
                        marks.Add(mark);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching marks: {ex.Message}");
                }
                finally
                {
                    db.Close_Connection();
                }
            }

            return marks;
        }

        //update Marks 
    }
}
