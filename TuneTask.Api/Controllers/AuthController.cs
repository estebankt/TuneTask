using Microsoft.AspNetCore.Mvc;
using TuneTask.Core.Entities;
using TuneTask.Core.Services;
using System.ComponentModel.DataAnnotations; // Import for data annotations

namespace TuneTask.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;

    public AuthController(AuthService authService, JwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="dto">The user registration data.</param>
    /// <returns>A success message.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">Bad request if registration fails (e.g., duplicate email).</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var success = await _authService.RegisterAsync(dto.Username, dto.Email, dto.Password, dto.Role);
        if (!success) return BadRequest("Registration failed.");

        return Ok(new { message = "User registered successfully." });
    }

    /// <summary>
    /// Login a user.
    /// </summary>
    /// <param name="dto">The user login credentials.</param>
    /// <returns>A JWT token.</returns>
    /// <response code="200">Returns the JWT token.</response>
    /// <response code="401">Unauthorized if login fails (invalid email or password).</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await _authService.LoginAsync(dto.Email, dto.Password);
        if (user == null) return Unauthorized("Invalid email or password.");

        var token = _jwtService.GenerateToken(user);
        return Ok(new { token });
    }
}

public class UserRegisterDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty; // Initialize to empty string

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")] // Example password validation
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; } = string.Empty;
}

public class UserLoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}