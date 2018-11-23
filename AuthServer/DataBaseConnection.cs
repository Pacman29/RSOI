
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public static class DataBaseConnection
    {
        
        static string providerName = "Npgsql";
        static string databaseName = "rsoi_database_users";
        static string userName = "rsoi";
        static string password = "rsoi";
        static string host = "localhost";
        static int port = 5432;
        
        public static void GetDatabaseConnection(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(GetDatabaseConnection());
        }
        
        public static string GetDatabaseConnection()
        {
            return $"Server={host}; "
                 + $"Port={port}; " 
                 + $"Username={userName};"
                 + $"Password={password};"
                 + $"Database={databaseName};";
        }
    }
}