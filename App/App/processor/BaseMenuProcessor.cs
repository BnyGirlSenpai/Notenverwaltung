using App.App.api;
using App.App.repositorys;

namespace App.App.processor
{
    internal class BaseMenuProcessor
    {
        protected static async Task<List<CourseRepository>> GetCoursesForUser(string userId, string connectionStatus, bool isTeacher)
        {
            return isTeacher
                ? await CourseApi.GetAllCoursesForTeacher(userId, connectionStatus)
                : await CourseApi.GetAllCoursesForStudent(userId, connectionStatus);
        }

        protected static async Task<List<StudentRepository>> GetStudentsForCourse(string userId, string courseId, string connectionStatus)
        {
            return await CourseApi.GetAllStudentsForCourse(userId, courseId, connectionStatus);
        }

        protected static async Task<List<LessonRepository>> GetLessonsForCourse(string userId, string courseId, string connectionStatus)
        {
            return await CourseApi.GetAllLessonsForCourse(userId, courseId, connectionStatus);
        }

        protected static async Task<List<MarkRepository>> GetMarksForLesson(string userId, string lessonId, string connectionStatus)
        {
            return await MarkApi.GetMarksForStudent(userId, lessonId, connectionStatus);
        }

        protected static async Task<List<AttendanceRepository>> GetAttendanceForLesson(string userId, string lessonId, string connectionStatus)
        {
            return await AttendanceApi.GetAttendanceForStudent(userId, lessonId, connectionStatus);
        }

        protected static async Task UpdateMarks(string studentId, string lessonId, string teacherId, string newTeacherMark, string newFinalMark, string connectionStatus)
        {
            await MarkApi.UpdateMarksAsTeacher(studentId, lessonId, teacherId, newTeacherMark, newFinalMark, connectionStatus);
        }

        protected static async Task UpdateAttendance(string userId, string lessonId, string newAttendanceStatus, string connectionStatus)
        {
            await AttendanceApi.UpdateAttendanceForStudent(userId, lessonId, newAttendanceStatus, connectionStatus);
        }

        public static int ShowMenu(string header, string[] options)
        {
            int selectedIndex = 0;
            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Clear();
                Console.WriteLine(header);
                Console.WriteLine(new string('-', header.Length));
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("-> " + options[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("   " + options[i]);
                    }
                }

                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = options.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex >= options.Length) selectedIndex = 0;
                }

            } while (keyInfo.Key != ConsoleKey.Enter);

            return selectedIndex;
        }
    }
}
