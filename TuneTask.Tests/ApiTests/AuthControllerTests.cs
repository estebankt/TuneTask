using System;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TuneTask.API.Controllers;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Core.Services;
using Xunit;

namespace TuneTask.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IJwtService> _jwtServiceMock;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _jwtServiceMock = new Mock<IJwtService>();

            _authController = new AuthController(_authServiceMock.Object, _jwtServiceMock.Object);
        }

        /// <summary>
        /// Tests successful user registration.
        /// </summary>
        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "SecurePass123",
                Role = "User"
            };

            _authServiceMock.Setup(service => service.RegisterAsync(
                registerDto.Username, registerDto.Email, registerDto.Password, registerDto.Role))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

        }

        /// <summary>
        /// Tests failed user registration due to duplicate email.
        /// </summary>
        [Fact]
        public async Task Register_EmailAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "existinguser@example.com",
                Password = "SecurePass123",
                Role = "User"
            };

            _authServiceMock.Setup(service => service.RegisterAsync(
                registerDto.Username, registerDto.Email, registerDto.Password, registerDto.Role))
                .ReturnsAsync(false);

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Registration failed.", badRequestResult.Value);
        }

        /// <summary>
        /// Tests successful login.
        /// </summary>
        [Fact]
        public async Task Login_ValidCredentials_ReturnsJwtToken()
        {
            // Arrange
            var loginDto = new UserLoginDto
            {
                Email = "testuser@example.com",
                Password = "SecurePass123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = loginDto.Email,
                Role = "User",
                PasswordHash = "HashedPassword123"
            };

            _authServiceMock.Setup(service => service.LoginAsync(loginDto.Email, loginDto.Password))
                .ReturnsAsync(user);

            _jwtServiceMock.Setup(service => service.GenerateToken(user))
                .Returns("mocked_jwt_token");

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var response = okResult.Value as dynamic;
            Assert.NotNull(response);

        }


        /// <summary>
        /// Tests login failure due to invalid credentials.
        /// </summary>
        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new UserLoginDto
            {
                Email = "testuser@example.com",
                Password = "WrongPassword"
            };

            _authServiceMock.Setup(service => service.LoginAsync(loginDto.Email, loginDto.Password))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Equal("Invalid email or password.", unauthorizedResult.Value);
        }
    }
}
