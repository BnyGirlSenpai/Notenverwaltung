using WebServer.Server.config;
using System.Data.Common;
using static WebServer.Server.config.Database;

namespace WebServer.Server.controllers
{
    internal abstract class BaseController
    {
        protected DbConnection connection;

        protected void ConnectToDatabase()
        {
            var db = new Database(DatabaseType.MySQL);
            db.Connect_to_Database();
            connection = (DbConnection)db.GetConnection();
        }

        protected void CloseConnection()
        {
            if (connection != null && connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        protected DbCommand CreateCommand(string query)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            return command;
        }

        protected static void AddParameter(DbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        protected static List<T> ExecuteReader<T>(DbCommand command, Func<DbDataReader, T> readFunc)
        {
            var resultList = new List<T>();

            try
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    resultList.Add(readFunc(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error executing query: {ex.Message}");
            }

            return resultList;
        }
    }
}
