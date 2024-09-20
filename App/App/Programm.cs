using App.App.processor;
using App.App.services;
using App.App.utils;

internal class Program
{
    static async Task Main()
    {
        bool isAuthenticated = await LoginService.LoginAsync();
        string ConnectionStatus = await LocalDatabaseService.IsServerConnectedAsync();

        var dbSyncService = new DatabaseSyncronisationService();
        Console.WriteLine("Starting database synchronization...");
        dbSyncService.SyncAllTables(); 
        Console.WriteLine("Database synchronization completed.");

        if (isAuthenticated)
        {
            var (role, firstName, lastName, userId) = await UserInfoExtracter.GetUserInfo();
            string header = $"Logged in as: ({ConnectionStatus}) {firstName} {lastName} ({role})";

            if (role == "Teacher")
            {
                await TeacherMenuProcessor.ShowTeacherMenu(header, userId);
            }
            else if (role == "Student")
            {
                await StudentMenuProcessor.ShowStudentMenu(header);
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

