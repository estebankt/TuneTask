using System.Threading.Tasks;
using TuneTask.Core.Entities;

namespace TuneTask.Core.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string username, string email, string password, string role);
    }
}
