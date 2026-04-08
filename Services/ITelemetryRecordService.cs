using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Telemetry;
using Microsoft.AspNetCore.Mvc;

namespace FleetTelemetryAPI.Services;

public interface ITelemetryRecordService
{
    Task<PaginatedResultDto<TelemetryRecordOutputDto>> GetTelemetryRecordByIdAsync(int vehicleId, PaginationQueryDto pagination, CancellationToken cancellationToken);

    Task<Result> CreateTelemetryRecordAsync(TelemetryRecordInputDto telemetryRecord);
}
