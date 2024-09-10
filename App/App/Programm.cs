using App.App.repositorys;
using App.App.services;
using App.App.utils;
using System;
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

                    await GetNotesForCourse(userId, courseCode);
                    break;
                case 2:
                    await GetNotesForDateAndLesson(userId);
                    break;
                case 3:
                    Console.WriteLine("Vorname:");
                    firstName = Console.ReadLine();
                    Console.WriteLine("Nachname:");
                    lastName = Console.ReadLine();

                    await GetAttendanceForUser(userId, firstName, lastName);
                    break;
                case 4:
                    await GetAttendanceForLesson(userId);
                    break;
                case 5:
                    await ShowCourseMenu(header, userId);
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

    private static async Task ShowCourseMenu(string header, string userId)
    {
        var courses = await GetAllCourses(userId);

        if (courses == null || courses.Count == 0)
        {
            Console.WriteLine("No courses available or an error occurred.");
            return;
        }

        var courseOptions = courses.Select(course => $"{course.CourseCode}: {course.CourseName}").ToList();
        courseOptions.Add("Return to Previous Menu");

        while (true)
        {
            int selectedIndex = ShowMenu(header, [.. courseOptions]);

            if (selectedIndex >= 0 && selectedIndex < courses.Count)
            {
                var selectedCourse = courses[selectedIndex];
                Console.WriteLine($"Selected Course: {selectedCourse.CourseName}\n");

                var students = await GetAllStudentsForCourse(userId, selectedCourse.CourseId);
                if (students != null && students.Count > 0)
                {
                    var studentOptions = students.Select((student, index) => $"{index + 1}. {student.FirstName} {student.LastName}").ToList();
                    studentOptions.Add("Return to Course Menu");

                    while (true)
                    {
                        int studentSelection = ShowMenu("Select a student:", [.. studentOptions]);

                        if (studentSelection >= 0 && studentSelection < students.Count)
                        {
                            var selectedStudent = students[studentSelection];
                            Console.WriteLine($"Selected Student: {selectedStudent.FirstName} {selectedStudent.LastName}\n");

                            var lessons = await GetAllLessonsForCourse(userId, selectedCourse.CourseId);
                            if (lessons != null && lessons.Count > 0)
                            {
                                var lessonOptions = new List<string>();

                                foreach (var (lesson, index) in lessons.Select((lesson, index) => (lesson, index)))
                                {
                                    var marks = await GetMarksForStudent(selectedStudent.UserId, lesson.LessonId);

                                    var mark = marks?.FirstOrDefault();

                                    var markText = mark != null
                                        ? $"Note Lehrer: {mark.TeacherMark} | Note Schüler: {mark.StudentMark} | End Note: {mark.FinalMark}"
                                        : "Marks not available";

                                    lessonOptions.Add($"{index + 1}. {lesson.LessonName} - {lesson.LessonDate}\n   Anwesenheit:\n   {markText}\n");
                                }

                                lessonOptions.Add("Return to Student Menu");

                                while (true)
                                {   
                                    int lessonSelection = ShowMenu($"Select a lesson for {selectedStudent.FirstName} {selectedStudent.LastName}:", [.. lessonOptions]);

                                    if (lessonSelection >= 0 && lessonSelection < lessons.Count)
                                    {
                                        var selectedLesson = lessons[lessonSelection];
                                        Console.WriteLine($"Selected Lesson: {selectedLesson.LessonName} on {selectedLesson.LessonDate}\n");

                                        // Here you can add additional functionality, such as displaying lesson details, etc.
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
                            }
                            else
                            {
                                Console.WriteLine("No lessons found for this course.");
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
                }
                else
                {
                    Console.WriteLine("No students found for this course.");
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


    static async Task<List<CourseRepository>> GetAllCourses(string userId)
    {
        Console.WriteLine("Fetching all courses...");
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/courses")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("userId", userId),
            ])
        };

        try
        {
            using HttpClient _client = new();
            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            var courses = JsonSerializer.Deserialize<List<CourseRepository>>(responseData);

            return courses;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
    }

    static async Task<List<LessonRepository>> GetAllLessonsForCourse(string userId, string courseId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/lessons")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("userId", userId),
                new KeyValuePair<string, string>("courseId", courseId)
            ])
        };

        try
        {
            using HttpClient _client = new();
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            var lessons = JsonSerializer.Deserialize<List<LessonRepository>>(responseData);

            return lessons;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
    }
    
    static async Task<List<StudentRepository>> GetAllStudentsForCourse(string userId, string courseId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/students")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("userId", userId),
                new KeyValuePair<string, string>("courseId", courseId),
            ])
        };

        try
        {
            using HttpClient _client = new();
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            var students = JsonSerializer.Deserialize<List<StudentRepository>>(responseData);

            return students;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
    }

    static async Task<List<MarkRepository>> GetMarksForStudent(string studentId, string lessonId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/lesson/student/marks")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("studentId", studentId),
                new KeyValuePair<string, string>("lessonId", lessonId),
            ])
        };

        try
        {
            using HttpClient _client = new();

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();

            // Log the raw response data
            Console.WriteLine($"Response data: {responseData}");

            var marks = JsonSerializer.Deserialize<List<MarkRepository>>(responseData);

            return marks;
        }
        catch (HttpRequestException e)
        {
            // Log the exception message
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
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

                string userId = userIdElement.GetString() ?? "Unknown"; 
                string firstName = firstNameElement.GetString() ?? "Unknown";
                string lastName = lastNameElement.GetString() ?? "Unknown";
                string role = roleElement.GetString() ?? "Unknown";

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

    static async Task<string> GetNotesForCourse(string userId, string courseId)
    {
        Console.WriteLine("Fetching notes for course...");

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/course")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("courseCode", courseId),
                new KeyValuePair<string, string>("userId", userId)
            ])
        };

        try
        {
            using HttpClient _client = new();
            {
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Course: {responseData}");
                return responseData;
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
    }

    static async Task<string> GetNotesForUser(string userId, string firstName, string lastName)
    {
        Console.WriteLine("Fetching notes for course...");

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api")
        {
            Content = new FormUrlEncodedContent(
            [
                 new KeyValuePair<string, string>("userId", userId),
                 new KeyValuePair<string, string>("courseCode", firstName),
                 new KeyValuePair<string, string>("courseCode", lastName),
            ])
        };

        try
        {
            using HttpClient _client = new();
            {
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Course: {responseData}");
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

    private static async Task<string> GetAttendanceForUser(string userId, string firstName, string lastName)
    {
        Console.WriteLine($"Attendance for {firstName} {lastName}...");

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/attendance")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("userId", userId),
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

    static async Task GetAttendanceForLesson(string userId)
    {
        Console.WriteLine("Fetching attendance for lesson...");
        // Simulate fetching attendance for a lesson.
        await Task.Delay(1000);
        Console.WriteLine("Attendance for lesson: [Example Attendance Data]");
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