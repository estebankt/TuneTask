using Microsoft.Data.SqlClient;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Infrastructure.Database;

namespace TuneTask.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _dbContext;

        public UserRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Private helper method to create and prepare a connection
        private async Task<SqlConnection> CreateConnectionAsync()
        {
            var connection = _dbContext.CreateConnection() as SqlConnection;
            if (connection == null)
                throw new InvalidOperationException("Failed to create a database connection.");

            await connection.OpenAsync();
            connection.ChangeDatabase("TuneTaskDB");
            return connection;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using (var connection = await CreateConnectionAsync())
            {
                using (var command = new SqlCommand("SELECT Id, Username, Email, PasswordHash FROM Users WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetGuid(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                PasswordHash = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<bool> CreateAsync(User user)
        {
            using (var connection = await CreateConnectionAsync())
            {
                using (var command = new SqlCommand("INSERT INTO Users (Id, Username, Email, PasswordHash) VALUES (@Id, @Username, @Email, @PasswordHash)", connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
    }
}
