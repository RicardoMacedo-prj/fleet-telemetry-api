using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Telemetry;
using Microsoft.AspNetCore.Mvc;

namespace FleetTelemetryAPI.Services;

public interface ITelemetryRecordService
{
    Task<PaginatedResultDto<TelemetryRecordOutputDto>> GetTelemetryRecordByIdAsync(int vehicleId, PaginationQueryDto pagination);

    Task<(bool IsSuccess, string ErrorMessage)> CreateTelemetryRecordAsync(TelemetryRecordInputDto telemetryRecord);
}
