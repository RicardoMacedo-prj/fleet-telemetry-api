using System.ComponentModel.DataAnnotations;
using FleetTelemetryAPI.Models.Fleet;

namespace FleetTelemetryAPI.DTOs.Fleet;

public class VehicleInputDto
{
    [MaxLength(20, ErrorMessage = "Registration number cannot exceed 20 characters.")]
    public required string RegistrationNumber { get; set; }

    [MaxLength(50, ErrorMessage = "Brand cannot exceed 50 characters.")]
    public required string Brand { get; set; }

    [MaxLength(50, ErrorMessage = "Model cannot exceed 50 characters.")]
    public required string Model { get; set; }

    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
    public int Year { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Load capacity must be a non-negative integer.")]
    public int LoadCapacity { get; set; }
    public VehicleType Type { get; set; }

}
