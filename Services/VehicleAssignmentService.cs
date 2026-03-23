using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.Models;
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
                DriverName = vehicleAssignments.Driver.Name,
                VehicleId = vehicleAssignments.VehicleId,
                VehicleRegistrationNumber = vehicleAssignments.Vehicle.RegistrationNumber,
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

    public async Task<VehicleAssignmentOutputDto?> GetVehicleAssignmentByIdAsync(int id)
    {
        return await _context.VehicleAssignments
            .Where(va => va.Id == id)
            .Select(vehicleAssignment => new VehicleAssignmentOutputDto
            {
                Id = vehicleAssignment.Id,
                DriverId = vehicleAssignment.DriverId,
                DriverName = vehicleAssignment.Driver.Name,
                VehicleId = vehicleAssignment.VehicleId,
                VehicleRegistrationNumber = vehicleAssignment.Vehicle.RegistrationNumber,
                AssignmentDate = vehicleAssignment.AssignmentDate,
                ReturnDate = vehicleAssignment.ReturnDate,
                Status = vehicleAssignment.Status.ToString()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(bool IsSuccess, string ErrorMessage, VehicleAssignmentOutputDto? Data)> CreateVehicleAssignmentAsync(VehicleAssignmentInputDto vehicleAssignment)
    {
        DateTime AssignmentDate = DateTime.UtcNow;

        if (vehicleAssignment.ExpectedReturnDate != null && vehicleAssignment.ExpectedReturnDate < AssignmentDate)
        {
            return (false, "Bad Request: Invalid expected return date.", null);
        }

        var driver = await _context.Drivers.FindAsync(vehicleAssignment.DriverId);
        var vehicle = await _context.Vehicles.FindAsync(vehicleAssignment.VehicleId);

        if (driver == null || vehicle == null)
        {
            return (false, "Bad Request: Driver or Vehicle does not exist.", null);
        }

        if (!driver.IsActive)
        {
            return (false, "Bad Request: Cannot assign an inactive driver.", null);
        }

        if (vehicle.Status != VehicleStatus.Active)
        {
            return (false, $"Bad Request: Cannot assign vehicle. Current status is {vehicle.Status}.", null);
        }

        if ((int)driver.LicenseCategory != (int)vehicle.Type)
        {

            return (false, "Bad Request: Driver's license category does not match vehicle type.", null);
        }

        var driverAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.DriverId == vehicleAssignment.DriverId && va.ReturnDate == null);

        var vehicleAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.VehicleId == vehicleAssignment.VehicleId && va.ReturnDate == null);

        if (driverAlreadyAssigned)
        {
            return (false, "Conflict: Driver is already assigned to another vehicle.", null);
        }
        if (vehicleAlreadyAssigned)
        {
            return (false, "Conflict: Vehicle is already assigned to another driver.", null);
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

        return (true, string.Empty, outputVehicleAssignment);
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> UpdateVehicleAssignmentAsync(int id, VehicleAssignmentInputDto vehicleAssignment)
    {
        var AssignmentToUpdate = await _context.VehicleAssignments.FindAsync(id);

        if (AssignmentToUpdate == null)
        {
            return (false, "Not Found");
        }

        if (vehicleAssignment.ExpectedReturnDate != null &&
            vehicleAssignment.ExpectedReturnDate < AssignmentToUpdate.AssignmentDate)
        {
            return (false, "Bad Request: Invalid expected return date.");
        }

        var driver = await _context.Drivers.FindAsync(vehicleAssignment.DriverId);
        var vehicle = await _context.Vehicles.FindAsync(vehicleAssignment.VehicleId);

        if (driver == null || vehicle == null)
        {
            return (false, "Bad Request: Driver or Vehicle does not exist.");
        }

        if (!driver.IsActive)
        {
            return (false, "Bad Request: Cannot assign an inactive driver.");
        }

        if (vehicle.Status != VehicleStatus.Active)
        {
            return (false, $"Bad Request: Cannot assign vehicle. Current status is {vehicle.Status}.");
        }

        if ((int)driver.LicenseCategory != (int)vehicle.Type)
        {

            return (false, "Bad Request: Driver's license category does not match vehicle type.");
        }

        var driverAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.DriverId == vehicleAssignment.DriverId && va.Id != id && va.ReturnDate == null);

        var vehicleAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.VehicleId == vehicleAssignment.VehicleId && va.Id != id && va.ReturnDate == null);

        if (driverAlreadyAssigned)
        {
            return (false, "Conflict: Driver is already assigned to another vehicle.");
        }
        if (vehicleAlreadyAssigned)
        {
            return (false, "Conflict: Vehicle is already assigned to another driver.");
        }

        AssignmentToUpdate.DriverId = vehicleAssignment.DriverId;
        AssignmentToUpdate.VehicleId = vehicleAssignment.VehicleId;
        AssignmentToUpdate.ExpectedReturnDate = vehicleAssignment.ExpectedReturnDate;

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> ReturnVehicle(int id)
    {
        var assignment = await _context.VehicleAssignments.FindAsync(id);

        if (assignment == null)
        {
            return (false, "Not Found");
        }

        if (assignment.Status == AssignmentStatus.Completed)
        {
            return (false, "Conflict: Assignment is already Completed.");
        }

        assignment.ReturnDate = DateTime.UtcNow;
        assignment.Status = AssignmentStatus.Completed;

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }
}
