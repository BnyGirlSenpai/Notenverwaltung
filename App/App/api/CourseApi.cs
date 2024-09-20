using App.App.controller;
using App.App.repositorys;
using App.App.services;
using System.Text.Json;

namespace App.App.api
{
    internal class CourseApi
    {
        public static async Task<List<CourseRepository>> GetAllCourses(string userId)
        {
            string connectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

            if (connectionStatus.Equals("Offline", StringComparison.OrdinalIgnoreCase))
            {
                return await LocalCourseController.GetCoursesByTeacherAsync(userId);
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/courses")
                {
                    Content = new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("userId", userId)
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
                    return []; 
                }
            }
        }

        public static async Task<List<StudentRepository>> GetAllStudentsForCourse(string userId, string courseId)
        {
            string connectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

            if (connectionStatus.Equals("Offline", StringComparison.OrdinalIgnoreCase))
            {
                return await LocalCourseController.GetStudentsByCourseAsync(courseId);
            }
            else
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
        }

        public static async Task<List<LessonRepository>> GetAllLessonsForCourse(string userId, string courseId)
        {
            string connectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

            if (connectionStatus.Equals("Offline", StringComparison.OrdinalIgnoreCase))
            {
                return await LocalCourseController.GetAllLessonsForCourseAsync(courseId);
            }
            else
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
        }

        public static async Task<List<MarkRepository>> GetMarksForStudent(string studentId, string lessonId)
        {
            string connectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

            if (connectionStatus.Equals("Offline", StringComparison.OrdinalIgnoreCase))
            {
                return await LocalMarkController.GetMarksForLessonsAsync(studentId, lessonId);
            }
            else
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
        }

        public static async Task<List<AttendanceRepository>> GetAttendanceForStudent(string studentId, string lessonId)
        {
            string connectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

            if (connectionStatus.Equals("Offline", StringComparison.OrdinalIgnoreCase))
            {
                return await LocalAttendanceController.GetAttendanceForLessonAsync(studentId, lessonId);
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/lesson/student/attendance")
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
                    var attendances = JsonSerializer.Deserialize<List<AttendanceRepository>>(responseData);

                    return attendances;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return null;
                }
            }
        }

        public static async Task<string> UpdateMarksForStudent(string studentId, string teacherId, string lessonId, string newTeacherMark, string newFinalMark)
        {
            string connectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

            if (connectionStatus.Equals("Offline", StringComparison.OrdinalIgnoreCase))
            {
                return await LocalMarkController.UpdateMarkForLessonAsync(studentId, teacherId, lessonId, newTeacherMark, newFinalMark);
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Put, "http://localhost:5000/api/lesson/student/update/marks")
                {
                    Content = new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("studentId", studentId),
                        new KeyValuePair<string, string>("teacherId", teacherId),
                        new KeyValuePair<string, string>("lessonId", lessonId),
                        new KeyValuePair<string, string>("teacherMark", newTeacherMark ?? string.Empty),
                        new KeyValuePair<string, string>("finalMark", newFinalMark ?? string.Empty)
                    ])
                };

                try
                {
                    using HttpClient _client = new();

                    var response = await _client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    return responseData; 
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return $"Error: {e.Message}"; 
                }
            }          
        }

        public static async Task UpdateAttendanceForStudent(string userId, string lessonId, string newAttendanceStatus)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "http://localhost:5000/api/lesson/student/update/attendance")
            {
                Content = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>("studentId", userId ?? string.Empty),
                    new KeyValuePair<string, string>("lessonId", lessonId ?? string.Empty),
                    new KeyValuePair<string, string>("attendanceStatus", newAttendanceStatus ?? string.Empty)
                ])
            };

            try
            {
                using HttpClient _client = new();
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
}
