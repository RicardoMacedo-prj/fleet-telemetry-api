using System.ComponentModel.DataAnnotations;

namespace FleetTelemetryAPI.Models.Identity;

public class Employee
{
    public int Id { get; set; }

    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public required string Username { get; set; }

    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public RoleType Role { get; set; }
    public bool IsActive { get; set; } = true;

}

public enum RoleType
{
    Admin,
    Manager,
    Driver
}
