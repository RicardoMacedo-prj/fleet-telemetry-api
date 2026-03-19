using System.ComponentModel.DataAnnotations;

namespace FleetTelemetryAPI.DTOs;

public class VehicleAssignmentInputDto
{
    [Range(1, int.MaxValue, ErrorMessage = "DriverId must be a positive integer.")]
    public int DriverId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "VehicleId must be a positive integer.")]
    public int VehicleId { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
}
