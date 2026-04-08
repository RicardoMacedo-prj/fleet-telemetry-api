using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Fleet;
using Microsoft.AspNetCore.Mvc;

namespace FleetTelemetryAPI.Services;

public interface IDriverService
{
    Task<PaginatedResultDto<DriverOutputDto>> GetAllDriversAsync(PaginationQueryDto pagination);

    Task<Result<DriverOutputDto>> GetDriverByIdAsync(int id);

    Task<Result<DriverOutputDto>> CreateDriverAsync(DriverInputDto driver);

    Task<Result> UpdateDriverAsync(int id, DriverInputDto driver);

    Task<Result> DeleteDriverAsync(int id);
}
