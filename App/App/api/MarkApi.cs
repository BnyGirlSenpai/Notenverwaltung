using App.App.controller;
using App.App.repositorys;
using App.App.services;

namespace App.App.api
{
    internal class MarkApi : BaseApi
    {
        public static async Task<List<MarkRepository>> GetMarksForStudent(string studentId, string lessonId, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalMarkController.GetMarksForLessonsAsync(studentId, lessonId);
            }
            else
            {
                string responseData = await SendGetRequest("http://localhost:5000/api/lesson/student/marks",
                    new Dictionary<string, string>
                    {
                        { "studentId", studentId },
                        { "lessonId", lessonId }
                    });

                return await DeserializeJsonAsync<MarkRepository>(responseData);
            }
        }

        public static async Task<string> UpdateMarksAsTeacher(string studentId, string lessonId, string teacherId, string newTeacherMark, string newFinalMark, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalMarkController.UpdateMarksForLessonAsync(studentId, lessonId, teacherId, newTeacherMark, newFinalMark);
            }
            else
            {
                string responseData = await SendPutRequest("http://localhost:5000/api/lesson/teacher/update/marks",
                    new Dictionary<string, string>
                    {
                        { "studentId", studentId },
                        { "lessonId", lessonId },
                        { "teacherId", teacherId },
                        { "teacherMark", newTeacherMark ?? string.Empty },
                        { "finalMark", newFinalMark ?? string.Empty }
                    });

                var dbSyncService = new TeacherDatabaseSynchronisationService();
                await dbSyncService.SyncAllTablesFromMySqlToSqliteAsync();
                return responseData;
            }
        }

        public static async Task<string> UpdateMarksAsStudent(string studentId, string lessonId, string newStudentMark, string connectionStatus)
        {
            if (IsOffline(connectionStatus))
            {
                return await LocalMarkController.UpdateStudentMarkForLessonAsync(studentId, lessonId, newStudentMark);
            }
            else
            {
                string responseData = await SendPutRequest("http://localhost:5000/api/lesson/student/update/studentmarks",
                    new Dictionary<string, string>
                    {
                        { "studentId", studentId },
                        { "lessonId", lessonId },
                        { "studentMark", newStudentMark ?? string.Empty }
                    });

                var dbSyncService = new StudentDatabaseSyncronisationService();
                await dbSyncService.SyncStudentMarksFromMySqlToSqliteAsync();
                return responseData;
            }
        }
    }
}
