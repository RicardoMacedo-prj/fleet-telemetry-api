using FleetTelemetryAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FleetTelemetryAPI.Services;

public interface IVehicleAssignmentService
{
    Task<PaginatedResultDto<VehicleAssignmentOutputDto>> GetAllVehicleAssignmentsAsync(PaginationQueryDto pagination);

    Task<VehicleAssignmentOutputDto?> GetVehicleAssignmentByIdAsync(int id);

    Task<(bool IsSuccess, string ErrorMessage, VehicleAssignmentOutputDto? Data)> CreateVehicleAssignmentAsync(VehicleAssignmentInputDto vehicleAssignment);

    Task<(bool IsSuccess, string ErrorMessage)> UpdateVehicleAssignmentAsync(int id, VehicleAssignmentInputDto vehicleAssignment);

    Task<(bool IsSuccess, string ErrorMessage)> ReturnVehicle(int id);


}
