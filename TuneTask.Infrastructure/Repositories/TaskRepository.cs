using System.Data;
using Microsoft.Data.SqlClient;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Infrastructure.Database;

namespace TuneTask.Infrastructure.Repositories;

public class TaskRepository : IRepository<TaskItem>
{
    private readonly DatabaseContext _dbContext;

    public TaskRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        var tasks = new List<TaskItem>();

        using (var connection = _dbContext.CreateConnection() as SqlConnection)
        {
            if (connection == null) throw new InvalidOperationException("Failed to create a database connection.");

            await connection.OpenAsync();
            connection.ChangeDatabase("TuneTaskDB"); // Ensure the correct database is selected

            using (var command = new SqlCommand("SELECT * FROM Tasks", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    tasks.Add(new TaskItem
                    {
                        Id = reader.GetGuid(0),
                        UserId = reader.GetGuid(1),
                        Title = reader.GetString(2),
                        Description = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4),
                        Status = Enum.Parse<Core.Entities.TaskStatus>(reader.GetString(5))
                    });
                }
            }
        }
        return tasks;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        using (var connection = _dbContext.CreateConnection() as SqlConnection)
        {
            if (connection == null) throw new InvalidOperationException("Failed to create a database connection.");

            await connection.OpenAsync();
            connection.ChangeDatabase("TuneTaskDB"); // Ensure the correct database is selected

            using (var command = new SqlCommand("SELECT * FROM Tasks WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new TaskItem
                        {
                            Id = reader.GetGuid(0),
                            UserId = reader.GetGuid(1),
                            Title = reader.GetString(2),
                            Description = reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4),
                            Status = Enum.Parse<Core.Entities.TaskStatus>(reader.GetString(5))
                        };
                    }
                }
            }
        }
        return null;
    }

    public async Task<bool> AddAsync(TaskItem task)
    {
        using (var connection = _dbContext.CreateConnection() as SqlConnection)
        {
            if (connection == null) throw new InvalidOperationException("Failed to create a database connection.");

            await connection.OpenAsync();
            connection.ChangeDatabase("TuneTaskDB"); // Ensure the correct database is selected

            using (var command = new SqlCommand(
                "INSERT INTO Tasks (Id, UserId, Title, Description, CreatedAt, Status) VALUES (@Id, @UserId, @Title, @Description, @CreatedAt, @Status)",
                connection))
            {
                command.Parameters.AddWithValue("@Id", task.Id);
                command.Parameters.AddWithValue("@UserId", task.UserId);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@CreatedAt", task.CreatedAt);
                command.Parameters.AddWithValue("@Status", task.Status.ToString());

                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }

    public async Task<bool> UpdateAsync(TaskItem task)
    {
        using (var connection = _dbContext.CreateConnection() as SqlConnection)
        {
            if (connection == null) throw new InvalidOperationException("Failed to create a database connection.");

            await connection.OpenAsync();
            connection.ChangeDatabase("TuneTaskDB"); // Ensure the correct database is selected

            using (var command = new SqlCommand(
                "UPDATE Tasks SET Title = @Title, Description = @Description, Status = @Status WHERE Id = @Id",
                connection))
            {
                command.Parameters.AddWithValue("@Id", task.Id);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@Status", task.Status.ToString());

                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var connection = _dbContext.CreateConnection() as SqlConnection)
        {
            if (connection == null) throw new InvalidOperationException("Failed to create a database connection.");

            await connection.OpenAsync();
            connection.ChangeDatabase("TuneTaskDB"); // Ensure the correct database is selected

            using (var command = new SqlCommand("DELETE FROM Tasks WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
