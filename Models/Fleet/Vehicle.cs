using FleetTelemetryAPI.Models.Telemetry;

namespace FleetTelemetryAPI.Models.Fleet;

public class Vehicle
{
    public int Id { get; set; }
    public required string RegistrationNumber { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public int LoadCapacity { get; set; }
    public VehicleType Type { get; set; }
    public VehicleStatus Status { get; set; }
    public ICollection<VehicleAssignment> VehicleAssignments { get; set; } = new List<VehicleAssignment>();
    public ICollection<TelemetryRecord> TelemetryRecords { get; set; } = new List<TelemetryRecord>();

}

public enum VehicleType
{
    Light,
    Heavy,
    Motorcycle
}

public enum VehicleStatus
{
    Active,
    Inactive,
    Maintenance
}
