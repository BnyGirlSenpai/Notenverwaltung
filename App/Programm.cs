using NotenverwaltungsApp.App.services;
using NotenverwaltungsApp.App.utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

internal class Program
{
    static async Task Main()
    {
        bool isAuthenticated = await LoginAsync();

        if (isAuthenticated)
        {
            var (role, firstName, lastName) = await GetUserInfo();
            string header = $"Logged in as: {firstName} {lastName} ({role})";

            if (role == "Teacher")
            {
                await ShowTeacherMenu(header);
            }
            else if (role == "Student")
            {
                await ShowStudentMenu(header);
            }
            else
            {
                Console.WriteLine("Unknown role. Exiting the program...");
            }
        }
        else
        {
            Console.WriteLine("Authentication failed. Exiting the program...");
        }
    }

    private static async Task ShowTeacherMenu(string header)
    {
        while (true)
        {
            int selectedIndex = ShowMenu(header, new string[]
            {
                "Option 1: Get Notes for User",
                "Option 2: Get Notes for Course",
                "Option 3: Get Notes for Date and Lesson",
                "Option 4: Get Attendance for User",
                "Option 5: Get Attendance for Lesson",
                "Option 6: Get All Courses",
                "Option 7: Logout"
            });

            switch (selectedIndex)
            {
                case 0:
                    await GetNotesForUser();
                    break;
                case 1:
                    await GetNotesForCourse();
                    break;
                case 2:
                    await GetNotesForDateAndLesson();
                    break;
                case 3:
                    await GetAttendanceForUser();
                    break;
                case 4:
                    await GetAttendanceForLesson();
                    break;
                case 5:
                    await GetAllCourses();
                    break;
                case 6:
                    await LogoutAsync();
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

    private static async Task ShowStudentMenu(string header)
    {
        while (true)
        {
            int selectedIndex = ShowMenu(header, new string[]
            {
                "Option 1: Get Notes for User",
                "Option 2: Get Attendance for User",
                "Option 3: Logout"
            });

            switch (selectedIndex)
            {
                case 0:
                    await GetNotesForUser();
                    break;
                case 1:
                    await GetAttendanceForUser();
                    break;
                case 2:
                    await LogoutAsync();
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

    static int ShowMenu(string header, string[] options)
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

    private static async Task<(string role, string firstName, string lastName)> GetUserInfo()
    {
        string token = TokenService.LoadToken();
        string secretKey = "u3rZ8BaR5WzCnP7GdT3JPEFbL0hG5lWm5F0q9PT0Ri8=\r\n";
        var tokenExtractor = new TokenExtractor(secretKey);
        string userInfoJson = tokenExtractor.ExtractUserInfoFromToken(token);

        try
        {
            var userInfo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userInfoJson);

            if (userInfo != null)
            {
                userInfo.TryGetValue("FirstName", out JsonElement firstNameElement);
                userInfo.TryGetValue("LastName", out JsonElement lastNameElement);
                userInfo.TryGetValue("Role", out JsonElement roleElement);

                string role = roleElement.GetString();
                string firstName = firstNameElement.GetString();
                string lastName = lastNameElement.GetString();

                Console.WriteLine($"Name: {firstName} {lastName}");
                Console.WriteLine($"Logged in as {role}");

                return (role, firstName, lastName);
            }
            else
            {
                Console.WriteLine("Failed to parse user information.");
                return (string.Empty, string.Empty, string.Empty);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing user information: {ex.Message}");
            return (string.Empty, string.Empty, string.Empty);
        }
    }

    static async Task GetNotesForUser()
    {
        Console.WriteLine("Fetching notes for user...");
        // Simulate fetching notes for user.
        await Task.Delay(1000);
        Console.WriteLine("Notes for user: [Example Note Data]");
    }

    static async Task GetNotesForCourse()
    {
        Console.WriteLine("Fetching notes for course...");
        // Simulate fetching notes for a course.
        await Task.Delay(1000);
        Console.WriteLine("Notes for course: [Example Note Data]");
    }

    static async Task GetNotesForDateAndLesson()
    {
        Console.WriteLine("Fetching notes for a specific date and lesson...");
        // Simulate fetching notes for a date and lesson.
        await Task.Delay(1000);
        Console.WriteLine("Notes for date and lesson: [Example Note Data]");
    }

    static async Task GetAttendanceForUser()
    {
        Console.WriteLine("Fetching attendance for user...");
        // Simulate fetching attendance for a user.
        await Task.Delay(1000);
        Console.WriteLine("Attendance for user: [Example Attendance Data]");
    }

    static async Task GetAttendanceForLesson()
    {
        Console.WriteLine("Fetching attendance for lesson...");
        // Simulate fetching attendance for a lesson.
        await Task.Delay(1000);
        Console.WriteLine("Attendance for lesson: [Example Attendance Data]");
    }

    static async Task GetAllCourses()
    {
        Console.WriteLine("Fetching all courses...");
        // Simulate fetching all courses.
        await Task.Delay(1000);
        Console.WriteLine("All courses: [Example Course Data]");
    }

    private static async Task<bool> LoginAsync()
    {
        var authenticator = new Authenticator();
        bool authenticated = await authenticator.AuthenticateAsync();

        return authenticated;
    }

    private static async Task LogoutAsync()
    {
        try
        {
            TokenService.DeleteToken();
            Console.WriteLine("Abmeldung erfolgreich.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler bei der Abmeldung: {ex.Message}");
        }
    }
}