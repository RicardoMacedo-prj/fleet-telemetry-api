using FleetTelemetryAPI.Models;

namespace FleetTelemetryAPI.DTOs;

public class DriverInputDto
{
    public required string Name { get; set; }
    public required string LicenseNumber { get; set; }
    public LicenseCategory LicenseCategory { get; set; }
}
