using App.App.repositorys;

using System.Text.Json;

namespace App.App.services
{
    internal class CourseService
    {
        public static async Task<List<CourseRepository>> GetAllCourses(string userId)
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

        public static async Task<List<LessonRepository>> GetAllLessonsForCourse(string userId, string courseId)
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

        public static async Task<List<StudentRepository>> GetAllStudentsForCourse(string userId, string courseId)
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

        public static async Task<List<MarkRepository>> GetMarksForStudent(string studentId, string lessonId)
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
                var marks = JsonSerializer.Deserialize<List<MarkRepository>>(responseData);

                return marks;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }

        public static async Task<List<AttendanceRepository>> GetAttendanceForStudent(string studentId, string lessonId)
        {
            Console.WriteLine("Starting GetAttendanceForStudent method");
            Console.WriteLine($"Student ID: {studentId}, Lesson ID: {lessonId}");

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/lesson/student/attendance")
            {
                Content = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>("studentId", studentId),
                new KeyValuePair<string, string>("lessonId", lessonId),
            ])
            };

            Console.WriteLine("HTTP request created");

            try
            {
                using HttpClient _client = new();
                Console.WriteLine("Sending HTTP request...");

                var response = await _client.SendAsync(request);
                Console.WriteLine("HTTP request sent");

                response.EnsureSuccessStatusCode();
                Console.WriteLine("Response status code: " + response.StatusCode);

                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response data received:");
                Console.WriteLine(responseData);

                var attendances = JsonSerializer.Deserialize<List<AttendanceRepository>>(responseData);
                Console.WriteLine("Deserialization completed");

                return attendances;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }
    }
}
