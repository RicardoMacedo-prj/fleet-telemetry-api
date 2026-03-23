using FleetTelemetryAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FleetTelemetryAPI.Services;

public interface IDriverService
{
    Task<PaginatedResultDto<DriverOutputDto>> GetAllDriversAsync(PaginationQueryDto pagination);

    Task<DriverOutputDto?> GetDriverByIdAsync(int id);

    Task<(bool IsSuccess, string ErrorMessage, DriverOutputDto? Data)> CreateDriverAsync(DriverInputDto driver);

    Task<(bool IsSuccess, string ErrorMessage)> UpdateDriverAsync(int id, DriverInputDto driver);

    Task<bool> DeleteDriverAsync(int id);
}
