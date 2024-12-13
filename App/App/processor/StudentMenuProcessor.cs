using App.App.services;

namespace App.App.processor
{
    internal class StudentMenuProcessor : BaseMenuProcessor
    {
        public static async Task ShowStudentMenu(string header, string connectionStatus, string userId)
        {
            while (true)
            {
                string[] menuOptions =
                [
                    "Option 1: Get All Courses",
                    "Option 2: Get Notes for Course (not in use)",
                    "Option 3: Get Notes for Date and Lesson (not in use)",
                    "Option 4: Get Attendance (not in use)",
                    "Option 5: Get Attendance for Course (not in use)",
                    "Option 6: Get All Final Notes (not in use)",
                    "Option 7: Logout"
                ];

                int selectedIndex = ShowMenu(header, menuOptions);

                switch (selectedIndex)
                {
                    case 0:
                        await CourseMenuProcessor.ShowStudentCourseMenu(header, connectionStatus, userId);
                        break;

                    case 1:
                        // to be implemented

                        // Handle course notes (not currently in use)
                        Console.WriteLine("Enter Course Code:");
                        _ = Console.ReadLine();

                        // await GetNotesForCourse(userId, courseCode);
                        Console.WriteLine("This functionality is not yet implemented.");
                        break;

                    case 2:
                        // to be implemented

                        // Handle date-specific notes (not currently in use)
                        Console.WriteLine("Enter Date:");
                        _ = Console.ReadLine();

                        // await GetNotesForDate(userId, date);
                        Console.WriteLine("This functionality is not yet implemented.");
                        break;

                    case 3:
                        // to be implemented
                    case 4:
                        // to be implemented
                    case 5:
                        // to be implemented
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
