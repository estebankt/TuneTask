using System.Data;
using System.Text.Json;
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

    private async Task<SqlConnection> CreateConnection()
    {
        var connection = _dbContext.CreateConnection() as SqlConnection;
        if (connection == null)
            throw new InvalidOperationException("Failed to create a database connection.");

        await connection.OpenAsync();
        connection.ChangeDatabase("TuneTaskDB");
        return connection;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        var tasks = new List<TaskItem>();

        using (var connection = await CreateConnection())
        {
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
        using (var connection = await CreateConnection())
        {
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
        using (var connection = await CreateConnection())
        {
            using (var command = new SqlCommand(
                "INSERT INTO Tasks (Id, UserId, Title, Description, CreatedAt, Status, Embedding) " +
                "VALUES (@Id, @UserId, @Title, @Description, @CreatedAt, @Status, @Embedding)",
                connection))
            {
                command.Parameters.AddWithValue("@Id", task.Id);
                command.Parameters.AddWithValue("@UserId", task.UserId);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@CreatedAt", task.CreatedAt);
                command.Parameters.AddWithValue("@Status", task.Status.ToString());

                // Handle Embedding Properly
                var embeddingJson = task.Embedding != null ? JsonSerializer.Serialize(task.Embedding) : (object)DBNull.Value;
                command.Parameters.AddWithValue("@Embedding", embeddingJson);

                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }


    public async Task<bool> UpdateAsync(TaskItem task)
    {
        using (var connection = await CreateConnection())
        {
            using (var command = new SqlCommand(
                "UPDATE Tasks SET Title = @Title, Description = @Description, Status = @Status, Embedding = @Embedding WHERE Id = @Id",
                connection))
            {
                command.Parameters.AddWithValue("@Id", task.Id);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@Status", task.Status.ToString());

                // Handle Embedding Update
                var embeddingJson = task.Embedding != null ? JsonSerializer.Serialize(task.Embedding) : (object)DBNull.Value;
                command.Parameters.AddWithValue("@Embedding", embeddingJson);

                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var connection = await CreateConnection())
        {
            using (var command = new SqlCommand("DELETE FROM Tasks WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }

    public async Task<List<TaskItem>> SearchTasksAsync(float[] queryEmbedding, int topN = 5)
    {
        var tasks = new List<TaskItem>();

        using (var connection = await CreateConnection())
        {
            using (var command = new SqlCommand("SELECT Id, Title, Description, Status, Embedding FROM Tasks", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    // Fix: Ensure correct index mapping
                    var id = reader.GetGuid(0);
                    var title = reader.GetString(1);
                    var description = reader.GetString(2);
                    var statusString = reader.GetString(3); // Ensure index matches 'Status'
                    var embeddingJson = reader.IsDBNull(4) ? null : reader.GetString(4); // Handle NULL embedding

                    // Fix: Parse status correctly
                    if (!Enum.TryParse<Core.Entities.TaskStatus>(statusString, out var status))
                    {
                        continue; 
                    }

                    
                    float[]? taskEmbedding = null;
                    if (!string.IsNullOrEmpty(embeddingJson))
                    {
                        try
                        {
                            taskEmbedding = JsonSerializer.Deserialize<float[]>(embeddingJson);
                        }
                        catch (JsonException)
                        {
                            continue; // Skip if JSON is malformed
                        }
                    }

                    if (taskEmbedding != null)
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = id,
                            Title = title,
                            Description = description,
                            Status = status,
                            Embedding = taskEmbedding,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }

        // Fix: Order tasks correctly and take the top N results
        return tasks
            .OrderByDescending(t => CosineSimilarity(queryEmbedding, t.Embedding!))
            .Take(topN)
            .ToList();
    }


    private static float CosineSimilarity(float[] v1, float[] v2)
    {
        float dotProduct = v1.Zip(v2, (a, b) => a * b).Sum();
        float magnitude1 = (float)Math.Sqrt(v1.Sum(a => a * a));
        float magnitude2 = (float)Math.Sqrt(v2.Sum(b => b * b));
        return dotProduct / (magnitude1 * magnitude2);
    }
}