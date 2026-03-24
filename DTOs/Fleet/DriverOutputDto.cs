using FleetTelemetryAPI.Models.Fleet;

namespace FleetTelemetryAPI.DTOs.Fleet;

public class DriverOutputDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string LicenseNumber { get; set; }
    public LicenseCategory LicenseCategory { get; set; }
    public bool IsActive { get; set; }
}
