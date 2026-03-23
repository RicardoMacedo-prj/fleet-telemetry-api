using FleetTelemetryAPI.DTOs;

namespace FleetTelemetryAPI.Services;

public interface IVehicleService
{
    Task<PaginatedResultDto<VehicleOutputDto>> GetAllVehiclesAsync(PaginationQueryDto pagination);

    Task<VehicleOutputDto?> GetVehicleByIdAsync(int id);

    Task<(bool IsSuccess, string ErrorMessage, VehicleOutputDto? Data)> CreateVehicleAsync(VehicleInputDto vehicle);

    Task<(bool IsSuccess, string ErrorMessage)> UpdateVehicleStatusAsync(int id);

    Task<(bool IsSuccess, string ErrorMessage)> UpdateVehicleAsync(int id, VehicleInputDto vehicle);

    Task<bool> DeleteVehicleAsync(int id);
}
