using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Fleet;
using Microsoft.AspNetCore.Mvc;

namespace FleetTelemetryAPI.Services;

public interface IVehicleAssignmentService
{
    Task<PaginatedResultDto<VehicleAssignmentOutputDto>> GetAllVehicleAssignmentsAsync(PaginationQueryDto pagination);

    Task<Result<VehicleAssignmentOutputDto>> GetVehicleAssignmentByIdAsync(int id);

    Task<Result<VehicleAssignmentOutputDto>> CreateVehicleAssignmentAsync(VehicleAssignmentInputDto vehicleAssignment);

    Task<Result> UpdateVehicleAssignmentAsync(int id, VehicleAssignmentInputDto vehicleAssignment);

    Task<Result> ReturnVehicle(int id);


}
