using System.Threading.Tasks;
using Moq;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Core.Services;
using TuneTask.Shared.Exceptions;
using Xunit;

namespace TuneTask.Core.Services.Tests
{
    public class AuthServiceTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthService _authService;

        public AuthServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authService = new AuthService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_UserAlreadyExists_ThrowsUserAlreadyExistsException()
        {
            // Arrange
            var email = "existinguser@example.com";
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<UserAlreadyExistsException>(() => _authService.RegisterAsync("username", email, "password"));
        }

        [Fact]
        public async Task RegisterAsync_ValidUser_ReturnsTrue()
        {
            // Arrange
            var email = "newuser@example.com";
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync("username", email, "password");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ReturnsNull()
        {
            // Arrange
            var email = "nonexistentuser@example.com";
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync((User)null);

            // Act
            var result = await _authService.LoginAsync(email, "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var email = "user@example.com";
            var user = new User { Email = email, PasswordHash = "hashedpassword" };
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(email, "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var email = "user@example.com";
            var password = "password";
            var user = new User { Email = email, PasswordHash = _authService.HashPassword(password) };
        }
    }
}