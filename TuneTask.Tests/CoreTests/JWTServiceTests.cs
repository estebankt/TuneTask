using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using TuneTask.Core.Entities;
using Xunit;

namespace TuneTask.Core.Services.Tests
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly JwtService _jwtService;
        private readonly string _secretKey = "SuperSecureLongJwtKeyThatIsAtLeast32Characters";

        public JwtServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(config => config["Jwt:SecretKey"]).Returns(_secretKey);
            _jwtService = new JwtService(_configurationMock.Object);
        }

        [Fact]
        public void GenerateToken_ValidUser_ReturnsValidJwtToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                Role = "User"
            };

            // Act
            var token = _jwtService.GenerateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.False(string.IsNullOrWhiteSpace(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            // Validate Token
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            Assert.NotNull(validatedToken);
            Assert.IsType<JwtSecurityToken>(validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // Check Claims
            Assert.Equal(user.Id.ToString(), jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value);
            Assert.Equal(user.Username, jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
            Assert.Equal(user.Email, jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value);
            Assert.Equal(user.Role, jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value);
        }

        [Fact]
        public void GenerateToken_WithInvalidSecretKey_ThrowsException()
        {
            // Arrange
            var invalidSecretKey = "shortkey"; // Invalid, too short
            _configurationMock.Setup(config => config["Jwt:SecretKey"]).Returns(invalidSecretKey);
            var jwtServiceWithInvalidKey = new JwtService(_configurationMock.Object);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                Role = "User"
            };

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => jwtServiceWithInvalidKey.GenerateToken(user));
        }
    }
}
