using App.App.services;

namespace App.App.processor
{
    internal class StudentMenuProcessor
    {
        public static async Task ShowStudentMenu(string header)
        {
            while (true)
            {
                int selectedIndex = MenuProcessor.ShowMenu(header,
                [               
                    "Option 1: Logout"
                ]);

                switch (selectedIndex)
                {             
                    case 0:
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
