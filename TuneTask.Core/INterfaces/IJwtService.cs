using TuneTask.Core.Entities;

namespace TuneTask.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
