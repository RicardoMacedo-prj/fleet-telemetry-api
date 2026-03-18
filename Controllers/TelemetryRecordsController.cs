using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.Models;
using FleetTelemetryAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetTelemetryAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TelemetryRecordsController : ControllerBase
{
    private readonly FleetContext _context;

    public TelemetryRecordsController(FleetContext context)
    {
        _context = context;
    }


    // GET: api/TelemetryRecords/VehicleId
    [HttpGet("{VehicleId}")]
    public async Task<ActionResult> GetTelemetryRecordById(int vehicleId)
    {
        var telemetryRecord = await _context.TelemetryRecords
            .Where(tr => tr.VehicleId == vehicleId)
            .OrderByDescending(tr => tr.Timestamp)
            .Select(telemetryRecord => new TelemetryRecordOutputDto
            {
                Id = telemetryRecord.Id,
                VehicleId = telemetryRecord.VehicleId,
                Timestamp = telemetryRecord.Timestamp,
                Latitude = telemetryRecord.Latitude,
                Longitude = telemetryRecord.Longitude,
                Speed = telemetryRecord.Speed,
                FuelLevel = telemetryRecord.FuelLevel,
                FuelConsumptionRate = telemetryRecord.FuelConsumptionRate
            })
            .ToListAsync();

        if (!telemetryRecord.Any())
        {
            return NotFound("No records found.");
        }

        return Ok(telemetryRecord);
    }

    // POST: api/TelemetryRecords
    [HttpPost]
    public async Task<ActionResult> CreateTelemetryRecord(TelemetryRecordInputDto telemetryRecord)
    {

        var ValidVehicle = await _context.Vehicles
            .AnyAsync(v => v.Status == VehicleStatus.Active && v.Id == telemetryRecord.VehicleId);
        
        var AssignedVehicle = await _context.VehicleAssignments
            .AnyAsync(va => va.Status == AssignmentStatus.Active && va.VehicleId == telemetryRecord.VehicleId);

        if (!ValidVehicle)
        {
            return BadRequest("This vehicle does not exist or is inactive.");
        }

        if (!AssignedVehicle)
        {
            return BadRequest("This vehicle does not have an active assignment.");
        }

        var newRecord = new TelemetryRecord
        {
            VehicleId = telemetryRecord.VehicleId,
            Timestamp = DateTime.UtcNow,
            Latitude = telemetryRecord.Latitude,
            Longitude = telemetryRecord.Longitude,
            Speed = telemetryRecord.Speed,
            FuelLevel = telemetryRecord.FuelLevel,
            FuelConsumptionRate = telemetryRecord.FuelConsumptionRate
        };

        _context.TelemetryRecords.Add(newRecord);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
