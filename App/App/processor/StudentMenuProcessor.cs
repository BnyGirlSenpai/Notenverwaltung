using App.App.services;

namespace App.App.processor
{
    internal class StudentMenuProcessor
    {
        public static async Task ShowStudentMenu(string header, string connectionStatus, string userId)
        {
            while (true)
            {
                int selectedIndex = MenuProcessor.ShowMenu(header,
                [
                    "Option 1: Get All Courses",
                    "Option 2: Get Notes for Course (not in use)",
                    "Option 3: Get Notes for Date (not in use)",
                    "Option 4: Logout"
                ]);

                switch (selectedIndex)
                {
                    case 0:
                        await CourseMenuProcessor.ShowStudentCourseMenu(header, connectionStatus, userId);
                        break;
                    case 1:
                        Console.WriteLine("Course Code:");
                        var courseCode = Console.ReadLine();
                        //await GetNotesForCourse(userId, courseCode);
                        break;
                    case 2:                
                        Console.WriteLine("date:");
                        var date = Console.ReadLine();
                        //await GetNotesForDate(userId);
                        break;         
                    case 3:
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
