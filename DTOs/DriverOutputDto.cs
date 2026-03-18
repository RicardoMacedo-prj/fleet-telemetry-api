using FleetTelemetryAPI.Models;

namespace FleetTelemetryAPI.DTOs;

public class DriverOutputDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string LicenseNumber { get; set; }
    public LicenseCategory LicenseCategory { get; set; }
    public bool IsActive { get; set; }
}
