using App.App.api;
using App.App.repositorys;
using System.Data;

namespace App.App.processor
{
    internal class CourseMenuProcessor : BaseMenuProcessor
    {
        private static async Task<List<CourseRepository>> GetCoursesForTeacher(string userId, string connectionStatus)
        {
            return await CourseApi.GetAllCoursesForTeacher(userId, connectionStatus);
        }

        private static async Task<List<CourseRepository>> GetCoursesForStudent(string userId, string connectionStatus)
        {
            return await CourseApi.GetAllCoursesForStudent(userId, connectionStatus);
        }

        private static async Task<List<StudentRepository>> GetStudentsForCourse(string userId, string courseId, string connectionStatus)
        {
            return await CourseApi.GetAllStudentsForCourse(userId, courseId, connectionStatus);
        }

        private static async Task<List<LessonRepository>> GetLessonsForCourse(string userId, string courseId, string connectionStatus)
        {
            return await CourseApi.GetAllLessonsForCourse(userId, courseId, connectionStatus);
        }

        private static async Task UpdateMarks(string studentId, string lessonId, string teacherId, string newTeacherMark, string newFinalMark, string connectionStatus)
        {
            await MarkApi.UpdateMarksAsTeacher(studentId, lessonId, teacherId,  newTeacherMark, newFinalMark, connectionStatus);
        }

        private static async Task UpdateStudentMark(string studentId, string lessonId, string newStudentMark, string connectionStatus)
        {
            await MarkApi.UpdateMarksAsStudent(studentId, lessonId, newStudentMark, connectionStatus);
        }
        
        private static async Task UpdateAttendance(string userId, string lessonId, string newAttendanceStatus, string connectionStatus)
        {
            await AttendanceApi.UpdateAttendanceForStudent(userId, lessonId, newAttendanceStatus, connectionStatus);
        }

        public static async Task ShowStudentCourseMenu(string header, string connectionStatus, string userId)
        {
            while (true)
            {
                var courses = await GetCoursesForStudent(userId, connectionStatus);

                if (courses == null || courses.Count == 0)
                {
                    Console.WriteLine("No courses available or an error occurred.");
                    return;
                }

                var courseOptions = courses.Select(course => $"{course.CourseCode}: {course.CourseName}").ToList();
                courseOptions.Add("Return to previous Menu");

                int selectedIndex = BaseMenuProcessor.ShowMenu(header, [.. courseOptions]);

                if (selectedIndex >= 0 && selectedIndex < courses.Count)
                {
                    var selectedCourse = courses[selectedIndex];
                    Console.WriteLine($"Selected Course: {selectedCourse.CourseName}\n");

                    while (true)
                    {
                        var lessons = await GetLessonsForCourse(userId, selectedCourse.CourseId, connectionStatus);
                        if (lessons != null && lessons.Count > 0)
                        {
                            var lessonOptions = new List<string>();

                            foreach (var (lesson, index) in lessons.Select((lesson, index) => (lesson, index)))
                            {
                                var marks = await MarkApi.GetMarksForStudent(userId, lesson.LessonId, connectionStatus);
                                var mark = marks?.FirstOrDefault();
                                var markText = mark != null
                                    ? $"Note Lehrer: {mark.TeacherMark} | Note Schüler: {mark.StudentMark} | End Note: {mark.FinalMark}"
                                    : "No Marks available";

                                var attendances = await AttendanceApi.GetAttendanceForStudent(userId, lesson.LessonId, connectionStatus);
                                var attendance = attendances?.FirstOrDefault();
                                var attendanceText = attendance != null
                                    ? $"Anwesenheit : {attendance.Status}"
                                    : "No entry available";

                                lessonOptions.Add($"{index + 1}. {lesson.LessonName} - {lesson.LessonDate} - {lesson.LessonType}\n   {attendanceText}\n   {markText}\n");
                            }
                            lessonOptions.Add("Return to previous Menu");

                            int lessonSelection = BaseMenuProcessor.ShowMenu($"Select a lesson for :", [.. lessonOptions]);

                            if (lessonSelection >= 0 && lessonSelection < lessons.Count)
                            {
                                var selectedLesson = lessons[lessonSelection];
                                Console.WriteLine($"Selected Lesson: {selectedLesson.LessonName} on {selectedLesson.LessonDate}\n");

                                Console.WriteLine("Enter new student mark (or leave empty to skip): ");
                                var newStudentMark = Console.ReadLine();

                                if (!string.IsNullOrEmpty(newStudentMark))
                                {
                                    await UpdateStudentMark(userId, selectedLesson.LessonId, newStudentMark, connectionStatus);
                                }

                                lessons = await GetLessonsForCourse(userId, selectedCourse.CourseId, connectionStatus);
                            }
                            else if (lessonSelection == lessons.Count)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid selection. Please try again.");
                            }

                            Console.WriteLine("\nPress any key to return to previous Menu");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("No lessons found for this course.");
                            Console.ReadKey();
                            break;
                        }
                    }
                }
                else if (selectedIndex == courses.Count)
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }

        public static async Task ShowTeacherCourseMenu(string header, string userId, string connectionStatus)
        {
            while (true)
            {
                var courses = await GetCoursesForTeacher(userId, connectionStatus);
                
                if (courses == null || courses.Count == 0)
                {
                    Console.WriteLine("No courses available or an error occurred.");
                    return;
                }

                var courseOptions = courses.Select(course => $"{course.CourseCode}: {course.CourseName}").ToList();
                courseOptions.Add("Return to previous Menu");

                int selectedIndex = BaseMenuProcessor.ShowMenu(header, [..courseOptions]);

                if (selectedIndex >= 0 && selectedIndex < courses.Count)
                {
                    var selectedCourse = courses[selectedIndex];
                    Console.WriteLine($"Selected Course: {selectedCourse.CourseName}\n");

                    while (true)
                    {
                        var students = await GetStudentsForCourse(userId, selectedCourse.CourseId, connectionStatus);

                        if (students != null && students.Count > 0)
                        {
                            var studentOptions = students.Select((student, index) => $"{index + 1}. {student.FirstName} {student.LastName}").ToList();
                            studentOptions.Add("Return to previous Menu");

                            int studentSelection = BaseMenuProcessor.ShowMenu("Select a student:", [..studentOptions]);

                            if (studentSelection >= 0 && studentSelection < students.Count)
                            {
                                var selectedStudent = students[studentSelection];
                                Console.WriteLine($"Selected Student: {selectedStudent.FirstName} {selectedStudent.LastName}\n");

                                while (true)
                                {
                                    var lessons = await GetLessonsForCourse(userId, selectedCourse.CourseId, connectionStatus);
                                    if (lessons != null && lessons.Count > 0)
                                    {
                                        var lessonOptions = new List<string>();

                                        foreach (var (lesson, index) in lessons.Select((lesson, index) => (lesson, index)))
                                        {
                                            var marks = await MarkApi.GetMarksForStudent(selectedStudent.UserId, lesson.LessonId, connectionStatus);
                                            var mark = marks?.FirstOrDefault();
                                            var markText = mark != null
                                                ? $"Note Lehrer: {mark.TeacherMark} | Note Schüler: {mark.StudentMark} | End Note: {mark.FinalMark}"
                                                : "No Marks available";

                                            var attendances = await AttendanceApi.GetAttendanceForStudent(selectedStudent.UserId, lesson.LessonId, connectionStatus);
                                            var attendance = attendances?.FirstOrDefault();
                                            var attendanceText = attendance != null
                                                ? $"Anwesenheit : {attendance.Status}"
                                                : "No entry available";

                                            lessonOptions.Add($"{index + 1}. {lesson.LessonName} - {lesson.LessonDate} - {lesson.LessonType}\n   {attendanceText}\n   {markText}\n");
                                        }

                                        lessonOptions.Add("Return to previous Menu");

                                        int lessonSelection = BaseMenuProcessor.ShowMenu($"Select a lesson for {selectedStudent.FirstName} {selectedStudent.LastName}:", [..lessonOptions]);

                                        if (lessonSelection >= 0 && lessonSelection < lessons.Count)
                                        {
                                            var selectedLesson = lessons[lessonSelection];
                                            Console.WriteLine($"Selected Lesson: {selectedLesson.LessonName} on {selectedLesson.LessonDate}\n");

                                            Console.WriteLine("Enter new attendance status (Present, Absent, Excused) (or leave empty to skip): ");
                                            var newAttendanceStatus = Console.ReadLine();

                                            Console.WriteLine("Enter new teacher mark (or leave empty to skip): ");
                                            var newTeacherMark = Console.ReadLine();

                                            Console.WriteLine("Enter new final mark (or leave empty to skip): ");
                                            var newFinalMark = Console.ReadLine();

                                            if (!string.IsNullOrEmpty(newTeacherMark) || !string.IsNullOrEmpty(newFinalMark))
                                            {
                                                await UpdateMarks(selectedStudent.UserId, selectedLesson.LessonId, userId, newTeacherMark, newFinalMark, connectionStatus);
                                            }

                                            if (!string.IsNullOrEmpty(newAttendanceStatus))
                                            {
                                                await UpdateAttendance(selectedStudent.UserId, selectedLesson.LessonId, newAttendanceStatus, connectionStatus);
                                            }

                                            lessons = await GetLessonsForCourse(userId, selectedCourse.CourseId, connectionStatus);
                                        }
                                        else if (lessonSelection == lessons.Count)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid selection. Please try again.");
                                        }

                                        Console.WriteLine("\nPress any key to return to the previous menu...");
                                        Console.ReadKey();
                                    }
                                    else
                                    {
                                        Console.WriteLine("No lessons found for this course.");
                                        Console.ReadKey();
                                        break;
                                    }
                                }
                            }
                            else if (studentSelection == students.Count)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid selection. Please try again.");
                            }

                            Console.WriteLine("\nPress any key to return to the menu...");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("No students found for this course.");
                            Console.ReadKey();
                            break;                         
                        }
                    }
                }
                else if (selectedIndex == courses.Count)
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }
}
