using FleetTelemetryAPI.Models;

namespace FleetTelemetryAPI.DTOs;

public class VehicleInputDto
{
    public required string RegistrationNumber { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public int LoadCapacity { get; set; }
    public VehicleType Type { get; set; }

}
