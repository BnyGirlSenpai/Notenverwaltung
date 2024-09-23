using App.App.processor;
using App.App.services;
using App.App.utils;

internal class Program
{
    static async Task Main()
    {
        bool isAuthenticated = await LoginService.LoginAsync();
        string ConnectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

        if (isAuthenticated)
        {
            var (role, firstName, lastName, userId) = await UserInfoExtracter.GetUserInfo();
            string header = $"Logged in as: ({ConnectionStatus}) {firstName} {lastName} ({role})";

            if (role == "Teacher")
            {
                var dbSyncService = new TeacherDatabaseSyncronisationService();
                Console.WriteLine("Starting database synchronization...");
                await dbSyncService.SyncAllTablesFromSqliteToMySqlAsync();
                Console.WriteLine("Database synchronization completed.");
                await TeacherMenuProcessor.ShowTeacherMenu(header, userId);
            }
            else if (role == "Student")
            {
                var dbSyncService = new StudentDatabaseSyncronisationService();
                Console.WriteLine("Starting database synchronization...");
                await dbSyncService.SyncStudentMarksFromSqliteToMySqlAsync();
                Console.WriteLine("Database synchronization completed.");
                await StudentMenuProcessor.ShowStudentMenu(header, userId);
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

