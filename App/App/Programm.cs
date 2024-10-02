using App.App.processor;
using App.App.services;
using App.App.utils;

internal class Program
{
    static async Task Main()
    {
        bool isAuthenticated = await LoginService.LoginAsync();
        string connectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

        if (isAuthenticated)
        {
            var (role, firstName, lastName, userId) = await UserInfoExtracter.GetUserInfo();
            string header = $"Logged in as: ({connectionStatus}) {firstName} {lastName} ({role})";

            if (role == "Teacher")
            {
                if (connectionStatus == "Online")
                {
                    var dbSyncService = new TeacherDatabaseSynchronisationService();
                    Console.WriteLine("Starting database synchronization...");
                    await dbSyncService.SyncAllTablesFromSqliteToMySqlAsync();
                    await dbSyncService.SyncAllTablesFromMySqlToSqliteAsync();
                    Console.WriteLine("Database synchronization completed.");
                }
                await TeacherMenuProcessor.ShowTeacherMenu(header, connectionStatus, userId);
            }
            else if (role == "Student")
            {
                if (connectionStatus == "Online")
                {
                    var dbSyncService = new StudentDatabaseSyncronisationService();
                    Console.WriteLine("Starting database synchronization...");
                    await dbSyncService.SyncStudentMarksFromSqliteToMySqlAsync();
                    Console.WriteLine("Database synchronization completed.");
                }
                await StudentMenuProcessor.ShowStudentMenu(header, connectionStatus, userId);
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
}

