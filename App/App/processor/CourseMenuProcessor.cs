using App.App.repositorys;
using App.App.services;

namespace App.App.processor
{
    internal class CourseMenuProcessor
    {
        private static async Task<List<CourseRepository>> GetCourses(string userId)
        {
            return await CourseService.GetAllCourses(userId);
        }

        private static async Task<List<StudentRepository>> GetStudentsForCourse(string userId, string courseId)
        {
            return await CourseService.GetAllStudentsForCourse(userId, courseId);
        }

        private static async Task<List<LessonRepository>> GetLessonsForCourse(string userId, string courseId)
        {
            return await CourseService.GetAllLessonsForCourse(userId, courseId);
        }

        private static async Task UpdateMarks(string studentId, string teacherId, string lessonId, string newTeacherMark, string newFinalMark)
        {
            await CourseService.UpdateMarksForStudent(studentId, teacherId, lessonId, newTeacherMark, newFinalMark);
        }

        private static async Task UpdateAttendance(string userId, string lessonId, string newAttendanceStatus)
        {
            await CourseService.UpdateAttendanceForStudent(userId, lessonId, newAttendanceStatus);
        }

        public static async Task ShowCourseMenu(string header, string userId)
        {
            while (true)
            {
                var courses = await GetCourses(userId);
                
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
                                            var marks = await CourseService.GetMarksForStudent(selectedStudent.UserId, lesson.LessonId);
                                            var mark = marks?.FirstOrDefault();
                                            var markText = mark != null
                                                ? $"Note Lehrer: {mark.TeacherMark} | Note Schüler: {mark.StudentMark} | End Note: {mark.FinalMark}"
                                                : "No Marks available";

                                            var attendances = await CourseService.GetAttendanceForStudent(selectedStudent.UserId, lesson.LessonId);
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
