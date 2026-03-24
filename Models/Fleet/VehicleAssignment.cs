namespace FleetTelemetryAPI.Models.Fleet;

public class VehicleAssignment
{
    public int Id { get; set; }
    public Driver? Driver { get; set; }
    public int DriverId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public int VehicleId { get; set; }
    public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedReturnDate { get; set; }
    public DateTime? ReturnDate { get; set; } = null;
    public AssignmentStatus Status { get; set; }
}

public enum AssignmentStatus
{
    Active,
    Completed,
    Overdue
}
