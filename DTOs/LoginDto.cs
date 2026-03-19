using System.ComponentModel.DataAnnotations;

namespace FleetTelemetryAPI.DTOs;

public class LoginDto
{
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public required string Username { get; set; }

    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
    public required string Password { get; set; }
}
