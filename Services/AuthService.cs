using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Identity;
using FleetTelemetryAPI.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;

namespace FleetTelemetryAPI.Services;

public class AuthService : IAuthService
{
    private readonly AuthContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AuthContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<Result> RegisterAsync(RegisterDto register)
    {
        if (await _context.Employees.AnyAsync(e => e.Username == register.Username))
        {
            return Result.Failure(ErrorType.Validation, "Username already exists.");
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
        return Result.Success();
    }

    public async Task<Result<string>> LoginAsync(LoginDto login)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == login.Username);

        if (employee == null || !BCrypt.Net.BCrypt.Verify(login.Password, employee.PasswordHash))
        {
            return Result<string>.Failure(ErrorType.Unauthorized, "Invalid Credentials.");
        }

        if (!employee.IsActive)
        {
            return Result<string>.Failure(ErrorType.Unauthorized, "This account is disabled.");
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

        var tokenString =  new JwtSecurityTokenHandler().WriteToken(token);
        return Result<string>.Success(tokenString);
    }
}
