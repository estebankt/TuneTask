using Microsoft.AspNetCore.Mvc;
using TuneTask.Core.Entities;
using TuneTask.Core.Services;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var success = await _authService.RegisterAsync(dto.Username, dto.Email, dto.Password);
        if (!success) return BadRequest("Registration failed.");

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
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
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserLoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
