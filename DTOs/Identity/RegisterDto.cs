using System.ComponentModel.DataAnnotations;
using FleetTelemetryAPI.Models.Identity;

namespace FleetTelemetryAPI.DTOs.Identity;

public class RegisterDto
{
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public required string Username { get; set; }

    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public required string Email { get; set; }
    public required string Password { get; set; }
    public RoleType Role { get; set; }
}
