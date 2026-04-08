using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Fleet;

namespace FleetTelemetryAPI.Services;

public interface IVehicleService
{
    Task<PaginatedResultDto<VehicleOutputDto>> GetAllVehiclesAsync(PaginationQueryDto pagination);

    Task<Result<VehicleOutputDto>> GetVehicleByIdAsync(int id);

    Task<Result<VehicleOutputDto>> CreateVehicleAsync(VehicleInputDto vehicle);

    Task<Result> UpdateVehicleStatusAsync(int id);

    Task<Result> UpdateVehicleAsync(int id, VehicleInputDto vehicle);

    Task<Result> DeleteVehicleAsync(int id);
}
