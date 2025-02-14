using TuneTask.Core.Entities;

namespace TuneTask.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> CreateAsync(User user);
}
