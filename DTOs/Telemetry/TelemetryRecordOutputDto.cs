namespace FleetTelemetryAPI.DTOs.Telemetry;

public class TelemetryRecordOutputDto
{
    public long Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime Timestamp { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double FuelLevel { get; set; }
    public double FuelConsumptionRate { get; set; }

}
