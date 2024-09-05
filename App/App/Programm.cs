using App.App.services;
using App.App.utils;
using System.Text.Json;

internal class Program
{
    static async Task Main()
    {
        bool isAuthenticated = await LoginAsync();

        if (isAuthenticated)
        {
            var (role, firstName, lastName, userId) = await GetUserInfo();
            string header = $"Logged in as: {firstName} {lastName} ({role})";

            if (role == "Teacher")
            {
                await ShowTeacherMenu(header, userId);
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

    private static async Task ShowTeacherMenu(string header , string userId)
    {
        while (true)
        {
            int selectedIndex = ShowMenu(header,
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

                    await GetNotesForUser(userId, firstName, lastName);
                    break;
                case 1:
                    Console.WriteLine("Course Code:");
                    var courseCode = Console.ReadLine();

                    await GetNotesForCourse(courseCode);
                    break;
                case 2:
                    await GetNotesForDateAndLesson(userId);
                    break;
                case 3:
                    await GetAttendanceForUser(userId);
                    break;
                case 4:
                    await GetAttendanceForLesson(userId);
                    break;
                case 5:
                    await GetAllCourses(userId);
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
            int selectedIndex = ShowMenu(header,
            [       
                "Option 3: Logout"
            ]);

            switch (selectedIndex)
            {
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

    private static async Task<(string role, string firstName, string lastName, string userId)> GetUserInfo()
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
                userInfo.TryGetValue("UserId", out JsonElement userIdElement);

                string userId = userIdElement.GetString();
                string role = roleElement.GetString();
                string firstName = firstNameElement.GetString();
                string lastName = lastNameElement.GetString();

                Console.WriteLine($"User information:{userId}");
                Console.WriteLine($"Name: {firstName} {lastName}");
                Console.WriteLine($"Logged in as {role}");

                return (role, firstName, lastName, userId);
            }
            else
            {
                Console.WriteLine("Failed to parse user information.");
                return (string.Empty, string.Empty, string.Empty, string.Empty);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing user information: {ex.Message}");
            return (string.Empty, string.Empty, string.Empty, string.Empty);
        }
    }

    static async Task<string> GetNotesForUser(string userId, string firstName, string lastName)
    {
        Console.WriteLine($"Fetching notes for {firstName} {lastName}...");

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/notes")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName)
            ])
        };

        try
        {
            using HttpClient _client = new();
            {
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Notes for {firstName} {lastName}: {responseData}");
                return responseData;
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
    }

    static async Task<string> GetNotesForCourse(string courseCode)
    {
        Console.WriteLine("Fetching notes for course...");

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/course")
        {
            Content = new FormUrlEncodedContent(
             [
                 new KeyValuePair<string, string>("courseCode", courseCode),
             ])
        };

        try
        {
            using HttpClient _client = new();
            {
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Course {courseCode}: {responseData}");
                return responseData;
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
    }

    static async Task GetNotesForDateAndLesson(string userId)
    {
        Console.WriteLine("Fetching notes for a specific date and lesson...");
        // Simulate fetching notes for a date and lesson.
        await Task.Delay(1000);
        Console.WriteLine("Notes for date and lesson: [Example Note Data]");
    }

    static async Task GetAttendanceForUser(string userId)
    {
        Console.WriteLine("Fetching attendance for user...");
        // Simulate fetching attendance for a user.
        await Task.Delay(1000);
        Console.WriteLine("Attendance for user: [Example Attendance Data]");
    }

    static async Task GetAttendanceForLesson(string userId)
    {
        Console.WriteLine("Fetching attendance for lesson...");
        // Simulate fetching attendance for a lesson.
        await Task.Delay(1000);
        Console.WriteLine("Attendance for lesson: [Example Attendance Data]");
    }

    static async Task GetAllCourses(string userId)
    {
        Console.WriteLine("Fetching all courses...");
        // Simulate fetching all courses.
        await Task.Delay(1000);
        Console.WriteLine("All courses: [Example Course Data]");
    }

    private static async Task<bool> LoginAsync()
    {
        bool authenticated = await Authenticator.AuthenticateAsync();

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