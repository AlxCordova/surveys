using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Survey.API.SQL;

public static class Queries
{
    public static List<dynamic> ExecuteQuery(SqlConnection connection, string sqlCommand, CommandType type, object parameters)
    {
        try
        {
            return connection.Query(sqlCommand, parameters, commandType: type).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} . Exception: {e.Message}");
            return new List<dynamic>();
        }
    }
    public static int ExecuteNonQuery(SqlConnection connection, string sqlCommand, CommandType type, object parameters)
    {
        try
        {
            return connection.Execute(sqlCommand, parameters, commandType: type);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} . Exception: {e.Message}");
            return 0;
        }
    }

    public static List<T> ExecuteQuery<T>(SqlConnection connection, string sqlCommand, CommandType type, object parameters)
    {
        try
        {
            return connection.Query<T>(sqlCommand, parameters, commandType: type).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} . Exception: {e.Message}");
            return new List<T>();
        }
    }
}