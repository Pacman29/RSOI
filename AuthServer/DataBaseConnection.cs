
using Microsoft.EntityFrameworkCore;

namespace DataBaseServer
{
    public static class DataBaseConnection
    {
        
        static string providerName = "Npgsql";
        static string databaseName = "rsoi_database";
        static string userName = "rsoi";
        static string password = "rsoi";
        static string host = "localhost";
        static int port = 5432;
        
        public static DbContextOptions GetDatabaseConnection()
        {

            var dbContextOptions = new DbContextOptionsBuilder();
            dbContextOptions.UseNpgsql($"Server={host}; "
                                       + $"Port={port}; " 
                                       + $"Username={userName};"
                                       + $"Password={password};"
                                       + $"Database={databaseName};");

            if(dbContextOptions.IsConfigured)
                return dbContextOptions.Options;
            return null;
        }
        
        public static void GetDatabaseConnection(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql($"Server={host}; "
                                       + $"Port={port}; " 
                                       + $"Username={userName};"
                                       + $"Password={password};"
                                       + $"Database={databaseName};");
        }
    }
}