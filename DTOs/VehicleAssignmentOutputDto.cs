using FleetTelemetryAPI.Models;

namespace FleetTelemetryAPI.DTOs;

public class VehicleAssignmentOutputDto
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public required string DriverName { get; set; }
    public int VehicleId { get; set; }
    public required string VehicleRegistrationNumber { get; set; }
    public DateTime AssignmentDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public required string Status { get; set; }

}
