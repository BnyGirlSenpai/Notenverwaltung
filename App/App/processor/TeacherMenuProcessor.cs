using App.App.services;

namespace App.App.processor
{
    internal class TeacherMenuProcessor : BaseMenuProcessor
    {
        public static async Task ShowTeacherMenu(string header, string connectionStatus, string userId)
        {
            while (true)
            {
                string[] menuOptions =
                [
                    "Option 1: Get All Courses",
                    "Option 2: Get Notes for Course (not in use)",
                    "Option 3: Get Notes for Date and Lesson (not in use)",
                    "Option 4: Get Attendance for User (not in use)",
                    "Option 5: Get Attendance for Lesson (not in use)",
                    "Option 6: Get Notes for User (not in use)",
                    "Option 7: Logout"
                ];

                int selectedIndex = BaseMenuProcessor.ShowMenu(header, menuOptions);

                switch (selectedIndex)
                {
                    case 0:
                        await CourseMenuProcessor.ShowTeacherCourseMenu(header, userId, connectionStatus);
                        break;

                    case 1:
                        // Handle course notes (not currently in use)
                        Console.WriteLine("Enter Course Code:");
                        var courseCode = Console.ReadLine();
                        // await GetNotesForCourse(userId, courseCode);
                        Console.WriteLine("This functionality is not yet implemented.");
                        break;

                    case 2:
                        // Handle notes for date and lesson (not currently in use)
                        // await GetNotesForDateAndLesson(userId);
                        Console.WriteLine("This functionality is not yet implemented.");
                        break;

                    case 3:
                        // Handle attendance for a user (not currently in use)
                        Console.WriteLine("Enter First Name:");
                        var firstName = Console.ReadLine();
                        Console.WriteLine("Enter Last Name:");
                        var lastName = Console.ReadLine();
                        // await GetAttendanceForUser(userId, firstName, lastName);
                        Console.WriteLine("This functionality is not yet implemented.");
                        break;

                    case 4:
                        // Handle attendance for a lesson (not currently in use)
                        // await GetAttendanceForLesson(userId);
                        Console.WriteLine("This functionality is not yet implemented.");
                        break;

                    case 5:
                        // Handle notes for a user (not currently in use)
                        Console.WriteLine("Enter First Name:");
                        firstName = Console.ReadLine();
                        Console.WriteLine("Enter Last Name:");
                        lastName = Console.ReadLine();
                        // await GetNotesForUser(userId, firstName, lastName);
                        Console.WriteLine("This functionality is not yet implemented.");
                        break;

                    case 6:
                        await LoginService.LogoutAsync();
                        Console.WriteLine("You have been logged out. Exiting the program...");
                        return;

                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }
}
