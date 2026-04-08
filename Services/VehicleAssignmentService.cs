using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Fleet;
using FleetTelemetryAPI.Models.Fleet;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FleetTelemetryAPI.Services;

public class VehicleAssignmentService: IVehicleAssignmentService
{
    private readonly FleetContext _context;

    public VehicleAssignmentService(FleetContext context)
    {
        _context = context;
    }
    public async Task<PaginatedResultDto<VehicleAssignmentOutputDto>> GetAllVehicleAssignmentsAsync(PaginationQueryDto pagination)
    {
        var vehicleAssignmentQuery = _context.VehicleAssignments;
        var totalAssignemnts = await vehicleAssignmentQuery.CountAsync();

        var vehicleAssignments = await vehicleAssignmentQuery
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(vehicleAssignments => new VehicleAssignmentOutputDto
            {
                Id = vehicleAssignments.Id,
                DriverId = vehicleAssignments.DriverId,
                DriverName = vehicleAssignments.Driver!.Name,
                VehicleId = vehicleAssignments.VehicleId,
                VehicleRegistrationNumber = vehicleAssignments.Vehicle!.RegistrationNumber,
                AssignmentDate = vehicleAssignments.AssignmentDate,
                ReturnDate = vehicleAssignments.ReturnDate,
                Status = vehicleAssignments.Status.ToString()
            })
            .ToListAsync();

        return new PaginatedResultDto<VehicleAssignmentOutputDto>
        {
            TotalCount = totalAssignemnts,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Data = vehicleAssignments
        };
        
    }

    public async Task<Result<VehicleAssignmentOutputDto>> GetVehicleAssignmentByIdAsync(int id)
    {
        var assignment = await _context.VehicleAssignments
            .Where(va => va.Id == id)
            .Select(vehicleAssignment => new VehicleAssignmentOutputDto
            {
                Id = vehicleAssignment.Id,
                DriverId = vehicleAssignment.DriverId,
                DriverName = vehicleAssignment.Driver!.Name,
                VehicleId = vehicleAssignment.VehicleId,
                VehicleRegistrationNumber = vehicleAssignment.Vehicle!.RegistrationNumber,
                AssignmentDate = vehicleAssignment.AssignmentDate,
                ReturnDate = vehicleAssignment.ReturnDate,
                Status = vehicleAssignment.Status.ToString()
            })
            .FirstOrDefaultAsync();

        if (assignment == null)
        {
            return Result<VehicleAssignmentOutputDto>.Failure(ErrorType.NotFound, "Assignment not found.");
        }

        return Result<VehicleAssignmentOutputDto>.Success(assignment);
    }

    public async Task<Result<VehicleAssignmentOutputDto>> CreateVehicleAssignmentAsync(VehicleAssignmentInputDto vehicleAssignment)
    {
        DateTime AssignmentDate = DateTime.UtcNow;

        if (vehicleAssignment.ExpectedReturnDate != null && vehicleAssignment.ExpectedReturnDate < AssignmentDate)
        {
            return Result<VehicleAssignmentOutputDto>.Failure(ErrorType.Validation, "Invalid expected return date.");
        }

        var driver = await _context.Drivers.FindAsync(vehicleAssignment.DriverId);
        var vehicle = await _context.Vehicles.FindAsync(vehicleAssignment.VehicleId);

        if (driver == null || vehicle == null)
        {
            return Result<VehicleAssignmentOutputDto>.Failure(ErrorType.Validation, "Driver or Vehicle does not exist.");
        }

        if (!driver.IsActive)
        {
            return Result<VehicleAssignmentOutputDto>.Failure(ErrorType.Validation, "Cannot assign an inactive driver.");
        }

        if (vehicle.Status != VehicleStatus.Active)
        {
            return Result<VehicleAssignmentOutputDto>.Failure(ErrorType.Validation, $"Cannot assign vehicle. Current status is {vehicle.Status}.");
        }

        if ((int)driver.LicenseCategory != (int)vehicle.Type)
        {

            return Result<VehicleAssignmentOutputDto>.Failure(ErrorType.Validation, "Driver's license category does not match vehicle type.");
        }

        var driverAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.DriverId == vehicleAssignment.DriverId && va.ReturnDate == null);

        var vehicleAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.VehicleId == vehicleAssignment.VehicleId && va.ReturnDate == null);

        if (driverAlreadyAssigned)
        {
            return Result<VehicleAssignmentOutputDto>.Failure(ErrorType.Conflict, "Driver is already assigned to another vehicle.");
        }
        if (vehicleAlreadyAssigned)
        {
            return Result<VehicleAssignmentOutputDto>.Failure(ErrorType.Conflict, "Vehicle is already assigned to another driver.");
        }

        var newAssignment = new VehicleAssignment
        {
            DriverId = vehicleAssignment.DriverId,
            VehicleId = vehicleAssignment.VehicleId,
            ExpectedReturnDate = vehicleAssignment.ExpectedReturnDate,
            Status = AssignmentStatus.Active
        };

        _context.VehicleAssignments.Add(newAssignment);
        await _context.SaveChangesAsync();

        VehicleAssignmentOutputDto outputVehicleAssignment = new VehicleAssignmentOutputDto
        {
            Id = newAssignment.Id,
            DriverId = newAssignment.DriverId,
            DriverName = driver.Name,
            VehicleId = newAssignment.VehicleId,
            VehicleRegistrationNumber = vehicle.RegistrationNumber,
            AssignmentDate = newAssignment.AssignmentDate,
            ReturnDate = newAssignment.ReturnDate,
            Status = newAssignment.Status.ToString()

        };

        return Result<VehicleAssignmentOutputDto>.Success(outputVehicleAssignment);
    }

    public async Task<Result> UpdateVehicleAssignmentAsync(int id, VehicleAssignmentInputDto vehicleAssignment)
    {
        var AssignmentToUpdate = await _context.VehicleAssignments.FindAsync(id);

        if (AssignmentToUpdate == null)
        {
            return Result.Failure(ErrorType.NotFound, "Assignment not found.");
        }

        if (vehicleAssignment.ExpectedReturnDate != null &&
            vehicleAssignment.ExpectedReturnDate < AssignmentToUpdate.AssignmentDate)
        {
            return Result.Failure(ErrorType.Validation, "Invalid expected return date.");
        }

        var driver = await _context.Drivers.FindAsync(vehicleAssignment.DriverId);
        var vehicle = await _context.Vehicles.FindAsync(vehicleAssignment.VehicleId);

        if (driver == null || vehicle == null)
        {
            return Result.Failure(ErrorType.Validation, "Driver or Vehicle does not exist.");
        }

        if (!driver.IsActive)
        {
            return Result.Failure(ErrorType.Validation, "Cannot assign an inactive driver.");
        }

        if (vehicle.Status != VehicleStatus.Active)
        {
            return Result.Failure(ErrorType.Validation, $"Bad Request: Cannot assign vehicle. Current status is {vehicle.Status}.");
        }

        if ((int)driver.LicenseCategory != (int)vehicle.Type)
        {

            return Result.Failure(ErrorType.Validation, "Driver's license category does not match vehicle type.");
        }

        var driverAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.DriverId == vehicleAssignment.DriverId && va.Id != id && va.ReturnDate == null);

        var vehicleAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.VehicleId == vehicleAssignment.VehicleId && va.Id != id && va.ReturnDate == null);

        if (driverAlreadyAssigned)
        {
            return Result.Failure(ErrorType.Conflict, "Driver is already assigned to another vehicle.");
        }
        if (vehicleAlreadyAssigned)
        {
            return Result.Failure(ErrorType.Conflict, "Vehicle is already assigned to another driver.");
        }

        AssignmentToUpdate.DriverId = vehicleAssignment.DriverId;
        AssignmentToUpdate.VehicleId = vehicleAssignment.VehicleId;
        AssignmentToUpdate.ExpectedReturnDate = vehicleAssignment.ExpectedReturnDate;

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ReturnVehicle(int id)
    {
        var assignment = await _context.VehicleAssignments.FindAsync(id);

        if (assignment == null)
        {
            return Result.Failure(ErrorType.NotFound, "Assignment not found.");
        }

        if (assignment.Status == AssignmentStatus.Completed)
        {
            return Result.Failure(ErrorType.Conflict, "Assignment is already Completed.");
        }

        assignment.ReturnDate = DateTime.UtcNow;
        assignment.Status = AssignmentStatus.Completed;

        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
