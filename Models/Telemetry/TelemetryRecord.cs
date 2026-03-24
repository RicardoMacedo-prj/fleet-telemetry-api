using FleetTelemetryAPI.Models.Fleet;

namespace FleetTelemetryAPI.Models.Telemetry;

public class TelemetryRecord
{
    public long Id { get; set; }
    public Vehicle? Vehicle { get; set; }
    public int VehicleId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double FuelLevel { get; set; }
    public double FuelConsumptionRate { get; set; }

}
