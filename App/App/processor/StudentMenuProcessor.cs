using App.App.services;

namespace App.App.processor
{
    internal class StudentMenuProcessor
    {
        public static async Task ShowStudentMenu(string header, string userId)
        {
            while (true)
            {
                int selectedIndex = MenuProcessor.ShowMenu(header,
                [
                    "Option 1: Get All Courses",
                    "Option 2: Get Notes for Course",
                    "Option 3: Get Notes for Date and Lesson",
                    "Option 5: Get Attendance for Lesson",        
                    "Option 6: Logout"
                ]);

                switch (selectedIndex)
                {
                    case 0:
                        await CourseMenuProcessor.ShowStudentCourseMenu(header, userId);
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
                        var firstName = Console.ReadLine();
                        Console.WriteLine("Nachname:");
                        var lastName = Console.ReadLine();

                        //await GetAttendanceForUser(userId, firstName, lastName);
                        break;
                    case 4:
                        //await GetAttendanceForLesson(userId);
                        break;
                    case 5:
                        Console.WriteLine("Vorname:");
                        firstName = Console.ReadLine();

                        Console.WriteLine("Nachname:");
                        lastName = Console.ReadLine();

                        //await GetNotesForUser(userId, firstName, lastName);
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
