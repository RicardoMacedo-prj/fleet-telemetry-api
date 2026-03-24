namespace FleetTelemetryAPI.Models.Fleet;

public class Driver
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string LicenseNumber { get; set; }
    public LicenseCategory LicenseCategory { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<VehicleAssignment> VehicleAssignments { get; set; } = new List<VehicleAssignment>();


}

public enum LicenseCategory
{
    Light,
    Heavy,
    Motorcycle
}
