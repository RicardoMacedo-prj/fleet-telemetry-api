using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Telemetry;
using FleetTelemetryAPI.Models.Telemetry;
using FleetTelemetryAPI.Models.Fleet;
using Microsoft.EntityFrameworkCore;

namespace FleetTelemetryAPI.Services;

public class TelemetryRecordService: ITelemetryRecordService
{
    private readonly FleetContext _context;

    public TelemetryRecordService(FleetContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResultDto<TelemetryRecordOutputDto>> GetTelemetryRecordByIdAsync(int vehicleId, PaginationQueryDto pagination)
    {
        var telemetryQuery = _context.TelemetryRecords.Where(tr => tr.VehicleId == vehicleId);
        var totalRecords = await telemetryQuery.CountAsync();

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

        return new PaginatedResultDto<TelemetryRecordOutputDto>
        {
            TotalCount = totalRecords,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Data = telemetryRecord
        };
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> CreateTelemetryRecordAsync(TelemetryRecordInputDto telemetryRecord)
    {
        var hasActiveAssignment = await _context.VehicleAssignments
        .AnyAsync(va => va.VehicleId == telemetryRecord.VehicleId && va.Status == AssignmentStatus.Active);

        if (!hasActiveAssignment)
        {
            return (false, "Bad Request: Cannot accept telemetry. Vehicle has no active assignment or is inactive.");
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

        return (true, string.Empty);
    }
}
