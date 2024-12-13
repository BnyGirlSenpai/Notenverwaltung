using App.App.controller;
using App.App.repositorys;
using App.App.services;

namespace App.App.api
{
    internal class AttendanceApi : BaseApi
    {
        public static async Task<List<AttendanceRepository>> GetAttendanceForStudent(string studentId, string lessonId, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalAttendanceController.GetAttendanceForLessonAsync(studentId, lessonId);
            }
            else
            {
                string responseData = await SendGetRequest("http://localhost:5000/api/lesson/student/attendance",
                    new Dictionary<string, string>
                    {
                        { "studentId", studentId },
                        { "lessonId", lessonId }
                    });

                return await DeserializeJsonAsync<AttendanceRepository>(responseData);  
            }
        }

        public static async Task<string> UpdateAttendanceForStudent(string userId, string lessonId, string newAttendanceStatus, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalAttendanceController.UpdateAttendanceForLessonAsync(userId, lessonId, newAttendanceStatus);
            }
            else
            {
                string responseData = await SendPutRequest("http://localhost:5000/api/lesson/student/update/attendance",
                    new Dictionary<string, string>
                    {
                        { "studentId", userId ?? string.Empty },
                        { "lessonId", lessonId ?? string.Empty },
                        { "attendanceStatus", newAttendanceStatus ?? string.Empty }
                    });

                var dbSyncService = new TeacherDatabaseSynchronisationService();
                await dbSyncService.SyncAllTablesFromMySqlToSqliteAsync();
                return responseData;
            }
        }
    }
}
