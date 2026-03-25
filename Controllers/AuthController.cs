using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Identity;
using FleetTelemetryAPI.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FleetTelemetryAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AuthContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto register)
    {
        if (await _context.Employees.AnyAsync(e => e.Username == register.Username))
        {
            return BadRequest("Username already exists.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(register.Password);

        var employee = new Employee
        {
            Username = register.Username,
            Email = register.Email,
            PasswordHash = passwordHash,
            Role = register.Role
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully.");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == login.Username);

        if (employee == null || !BCrypt.Net.BCrypt.Verify(login.Password, employee.PasswordHash)) {
            return Unauthorized("Invalid Credentials.");
        }

        if (!employee.IsActive)
        {
            return Unauthorized("This account is disabled.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Username),
            new Claim(ClaimTypes.Role, employee.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"]!)),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}
