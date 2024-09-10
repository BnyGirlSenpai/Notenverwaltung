using App.App.services;

namespace App.App.processor
{
    internal class TeacherMenuProcessor
    {
        public static async Task ShowTeacherMenu(string header, string userId)
        {
            while (true)
            {
                int selectedIndex = MenuProcessor.ShowMenu(header,
                [
                    "Option 1: Get Notes for User",
                    "Option 2: Get Notes for Course",
                    "Option 3: Get Notes for Date and Lesson",
                    "Option 4: Get Attendance for User",
                    "Option 5: Get Attendance for Lesson",
                    "Option 6: Get All Courses",
                    "Option 7: Logout"
                ]);

                switch (selectedIndex)
                {
                    case 0:
                        Console.WriteLine("Vorname:");
                        var firstName = Console.ReadLine();
                        Console.WriteLine("Nachname:");
                        var lastName = Console.ReadLine();

                        //await GetNotesForUser(userId, firstName, lastName);
                        break;
                    case 1:
                        Console.WriteLine("Course Code:");
                        var courseCode = Console.ReadLine();

                        //await GetNotesForCourse(userId, courseCode);
                        break;
                    case 2:
                        //await GetNotesForDateAndLesson(userId);
                        break;
                    case 3:
                        Console.WriteLine("Vorname:");
                        firstName = Console.ReadLine();
                        Console.WriteLine("Nachname:");
                        lastName = Console.ReadLine();

                        //await GetAttendanceForUser(userId, firstName, lastName);
                        break;
                    case 4:
                        //await GetAttendanceForLesson(userId);
                        break;
                    case 5:
                        await CourseMenuProcessor.ShowCourseMenu(header, userId);
                        break;
                    case 6:
                        await LoginService.LogoutAsync();
                        Console.WriteLine("You have been logged out. Exiting the program...");
                        return;
                    default:
                        Console.WriteLine("Invalid selection.");
                        break;
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }
}
