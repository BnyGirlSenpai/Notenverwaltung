using App.App.api;
using App.App.repositorys;

namespace App.App.processor
{
    internal class CourseMenuProcessor
    {
        private static async Task<List<CourseRepository>> GetCoursesForTeacher(string userId)
        {
            return await CourseApi.GetAllCoursesForTeacher(userId);
        }

        private static async Task<List<CourseRepository>> GetCoursesForStudent(string userId)
        {
            return await CourseApi.GetAllCoursesForStudent(userId);
        }

        private static async Task<List<StudentRepository>> GetStudentsForCourse(string userId, string courseId)
        {
            return await CourseApi.GetAllStudentsForCourse(userId, courseId);
        }

        private static async Task<List<LessonRepository>> GetLessonsForCourse(string userId, string courseId)
        {
            return await CourseApi.GetAllLessonsForCourse(userId, courseId);
        }

        private static async Task UpdateMarks(string studentId, string teacherId, string lessonId, string newTeacherMark, string newFinalMark)
        {
            await CourseApi.UpdateMarksAsTeacher(studentId, teacherId, lessonId, newTeacherMark, newFinalMark);
        }
        private static async Task UpdateStudentMark(string studentId, string lessonId, string newStudentMark)
        {
            await CourseApi.UpdateMarksAsStudent(studentId, lessonId, newStudentMark);
        }
        
        private static async Task UpdateAttendance(string userId, string lessonId, string newAttendanceStatus)
        {
            await CourseApi.UpdateAttendanceForStudent(userId, lessonId, newAttendanceStatus);
        }

        public static async Task ShowStudentCourseMenu(string header, string userId)
        {
            while (true)
            {
                var courses = await GetCoursesForStudent(userId);

                if (courses == null || courses.Count == 0)
                {
                    Console.WriteLine("No courses available or an error occurred.");
                    return;
                }

                var courseOptions = courses.Select(course => $"{course.CourseCode}: {course.CourseName}").ToList();
                courseOptions.Add("Return to Previous Menu");

                int selectedIndex = MenuProcessor.ShowMenu(header, [.. courseOptions]);

                if (selectedIndex >= 0 && selectedIndex < courses.Count)
                {
                    var selectedCourse = courses[selectedIndex];
                    Console.WriteLine($"Selected Course: {selectedCourse.CourseName}\n");

                    while (true)
                    {
                        var lessons = await GetLessonsForCourse(userId, selectedCourse.CourseId);
                        if (lessons != null && lessons.Count > 0)
                        {
                            var lessonOptions = new List<string>();

                            foreach (var (lesson, index) in lessons.Select((lesson, index) => (lesson, index)))
                            {
                                var marks = await CourseApi.GetMarksForStudent(userId, lesson.LessonId);
                                var mark = marks?.FirstOrDefault();
                                var markText = mark != null
                                    ? $"Note Lehrer: {mark.TeacherMark} | Note Schüler: {mark.StudentMark} | End Note: {mark.FinalMark}"
                                    : "No Marks available";

                                var attendances = await CourseApi.GetAttendanceForStudent(userId, lesson.LessonId);
                                var attendance = attendances?.FirstOrDefault();
                                var attendanceText = attendance != null
                                    ? $"Anwesenheit : {attendance.Status}"
                                    : "No entry available";

                                lessonOptions.Add($"{index + 1}. {lesson.LessonName} - {lesson.LessonDate}\n   {attendanceText}\n   {markText}\n");
                            }

                            lessonOptions.Add("Return to Student Menu");

                            int lessonSelection = MenuProcessor.ShowMenu($"Select a lesson for :", [.. lessonOptions]);

                            if (lessonSelection >= 0 && lessonSelection < lessons.Count)
                            {
                                var selectedLesson = lessons[lessonSelection];
                                Console.WriteLine($"Selected Lesson: {selectedLesson.LessonName} on {selectedLesson.LessonDate}\n");

                                Console.WriteLine("Enter new student mark (or leave empty to skip): ");
                                var newStudentMark = Console.ReadLine();

                                if (!string.IsNullOrEmpty(newStudentMark))
                                {
                                    await UpdateStudentMark(userId, selectedLesson.LessonId, newStudentMark);                              
                                }

                                lessons = await GetLessonsForCourse(userId, selectedCourse.CourseId);
                            }
                            else if (lessonSelection == lessons.Count)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid selection. Please try again.");
                            }

                            Console.WriteLine("\nPress any key to return to the lesson menu...");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("No lessons found for this course.");
                        }
                    }
                }
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }

        public static async Task ShowTeacherCourseMenu(string header, string userId)
        {
            while (true)
            {
                var courses = await GetCoursesForTeacher(userId);
                
                if (courses == null || courses.Count == 0)
                {
                    Console.WriteLine("No courses available or an error occurred.");
                    return;
                }

                var courseOptions = courses.Select(course => $"{course.CourseCode}: {course.CourseName}").ToList();
                courseOptions.Add("Return to Previous Menu");

                int selectedIndex = MenuProcessor.ShowMenu(header, [..courseOptions]);

                if (selectedIndex >= 0 && selectedIndex < courses.Count)
                {
                    var selectedCourse = courses[selectedIndex];
                    Console.WriteLine($"Selected Course: {selectedCourse.CourseName}\n");

                    while (true)
                    {
                        var students = await GetStudentsForCourse(userId, selectedCourse.CourseId);

                        if (students != null && students.Count > 0)
                        {
                            var studentOptions = students.Select((student, index) => $"{index + 1}. {student.FirstName} {student.LastName}").ToList();
                            studentOptions.Add("Return to Course Menu");

                            int studentSelection = MenuProcessor.ShowMenu("Select a student:", [..studentOptions]);

                            if (studentSelection >= 0 && studentSelection < students.Count)
                            {
                                var selectedStudent = students[studentSelection];
                                Console.WriteLine($"Selected Student: {selectedStudent.FirstName} {selectedStudent.LastName}\n");

                                while (true)
                                {
                                    var lessons = await GetLessonsForCourse(userId, selectedCourse.CourseId);
                                    if (lessons != null && lessons.Count > 0)
                                    {
                                        var lessonOptions = new List<string>();

                                        foreach (var (lesson, index) in lessons.Select((lesson, index) => (lesson, index)))
                                        {
                                            var marks = await CourseApi.GetMarksForStudent(selectedStudent.UserId, lesson.LessonId);
                                            var mark = marks?.FirstOrDefault();
                                            var markText = mark != null
                                                ? $"Note Lehrer: {mark.TeacherMark} | Note Schüler: {mark.StudentMark} | End Note: {mark.FinalMark}"
                                                : "No Marks available";

                                            var attendances = await CourseApi.GetAttendanceForStudent(selectedStudent.UserId, lesson.LessonId);
                                            var attendance = attendances?.FirstOrDefault();
                                            var attendanceText = attendance != null
                                                ? $"Anwesenheit : {attendance.Status}"
                                                : "No entry available";

                                            lessonOptions.Add($"{index + 1}. {lesson.LessonName} - {lesson.LessonDate}\n   {attendanceText}\n   {markText}\n");
                                        }

                                        lessonOptions.Add("Return to Student Menu");

                                        int lessonSelection = MenuProcessor.ShowMenu($"Select a lesson for {selectedStudent.FirstName} {selectedStudent.LastName}:", [..lessonOptions]);

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
                                                await UpdateMarks(selectedStudent.UserId, selectedLesson.LessonId, userId, newTeacherMark, newFinalMark);
                                            }

                                            if (!string.IsNullOrEmpty(newAttendanceStatus))
                                            {
                                                await UpdateAttendance(selectedStudent.UserId, selectedLesson.LessonId, newAttendanceStatus);
                                            }

                                            lessons = await GetLessonsForCourse(userId, selectedCourse.CourseId);
                                        }
                                        else if (lessonSelection == lessons.Count)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid selection. Please try again.");
                                        }

                                        Console.WriteLine("\nPress any key to return to the lesson menu...");
                                        Console.ReadKey();
                                    }
                                    else
                                    {
                                        Console.WriteLine("No lessons found for this course.");
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
