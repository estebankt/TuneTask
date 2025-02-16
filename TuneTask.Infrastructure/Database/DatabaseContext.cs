using Microsoft.Data.SqlClient;
using System.Data;

namespace TuneTask.Infrastructure.Database;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
        EnsureDatabaseCreated();
    }

    private void EnsureDatabaseCreated()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                // Create database if it does not exist
                using (var command = new SqlCommand(
                    "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TuneTaskDB') " +
                    "CREATE DATABASE TuneTaskDB;", connection))
                {
                    command.ExecuteNonQuery();
                }

                // Switch to TuneTaskDB
                connection.ChangeDatabase("TuneTaskDB");

                // Create Users Table if it does not exist
                using (var command = new SqlCommand(
                    "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users') " +
                    "CREATE TABLE Users ( " +
                    "Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), " +
                    "Username NVARCHAR(100) NOT NULL, " +
                    "Email NVARCHAR(100) NOT NULL, " +
                    "Role NVARCHAR(100) NOT NULL, " +
                    "PasswordHash NVARCHAR(255) NOT NULL);", connection))
                {
                    command.ExecuteNonQuery();
                }

                // Create Tasks Table if it does not exist
                using (var command = new SqlCommand(
                    "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tasks') " +
                    "CREATE TABLE Tasks ( " +
                    "Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), " +
                    "UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id), " +
                    "Title NVARCHAR(255) NOT NULL, " +
                    "Description NVARCHAR(MAX), " +
                    "CreatedAt DATETIME DEFAULT GETDATE(), " +
                    "Status NVARCHAR(50) CHECK (Status IN ('Pending', 'In Progress', 'Completed')) NOT NULL DEFAULT 'Pending', " +
                    "Embedding NVARCHAR(MAX) NULL);", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }
    }

    public virtual IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
