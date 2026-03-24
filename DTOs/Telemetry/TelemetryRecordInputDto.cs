using System.ComponentModel.DataAnnotations;

namespace FleetTelemetryAPI.DTOs.Telemetry;

public class TelemetryRecordInputDto
{
    public int VehicleId { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public double Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public double Longitude { get; set; }

    [Range(0, 200, ErrorMessage = "Speed must be between 0 and 200 km/h.")]
    public double Speed { get; set; }

    [Range(0, 100, ErrorMessage = "Fuel level must be between 0 and 100%.")]
    public double FuelLevel { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Fuel consumption rate must be non-negative.")]
    public double FuelConsumptionRate { get; set; }
}
