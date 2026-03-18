namespace FleetTelemetryAPI.DTOs;

public class TelemetryRecordInputDto
{
    public int VehicleId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double FuelLevel { get; set; }
    public double FuelConsumptionRate { get; set; }
}
