using System.ComponentModel.DataAnnotations;
using FleetTelemetryAPI.Models;

namespace FleetTelemetryAPI.DTOs;

public class DriverInputDto
{
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public required string Name { get; set; }

    [MaxLength(20, ErrorMessage = "License number cannot exceed 20 characters.")]
    public required string LicenseNumber { get; set; }

    public LicenseCategory LicenseCategory { get; set; }
}
