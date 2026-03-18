namespace FleetTelemetryAPI.DTOs;

public class VehicleAssignmentInputDto
{
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
}
