using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.Models;
using FleetTelemetryAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FleetTelemetryAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class TelemetryRecordsController : ControllerBase
{
    private readonly FleetContext _context;

    public TelemetryRecordsController(FleetContext context)
    {
        _context = context;
    }


    // GET: api/TelemetryRecords/VehicleId
    [HttpGet("{VehicleId}")]
    public async Task<ActionResult> GetTelemetryRecordById([FromRoute] int vehicleId, [FromQuery] PaginationQueryDto pagination)
    {
        var telemetryQuery = _context.TelemetryRecords.Where(tr => tr.VehicleId == vehicleId);
        var totalRecords = await telemetryQuery.CountAsync();

        if (totalRecords == 0)
        {
            return NotFound("No records found.");
        }

        var telemetryRecord = await telemetryQuery
            .OrderByDescending(tr => tr.Timestamp)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
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

        var result = new PaginatedResultDto<TelemetryRecordOutputDto>
        {
            TotalCount = totalRecords,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Data = telemetryRecord
        };

        return Ok(result);
    }

    // POST: api/TelemetryRecords
    [HttpPost]
    public async Task<ActionResult> CreateTelemetryRecord([FromBody] TelemetryRecordInputDto telemetryRecord)
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
