using System.Data;
using System.Security.Cryptography;
using System.Text;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Shared.Exceptions;

namespace TuneTask.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> RegisterAsync(string username, string email, string password, string role = "User")
    {
        if (await _userRepository.GetByEmailAsync(email) != null)
            throw new UserAlreadyExistsException(email); 

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            Role = role
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        return HashPassword(password) == storedHash;
    }
}
