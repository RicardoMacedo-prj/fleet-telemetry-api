using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Fleet;
using FleetTelemetryAPI.Models.Fleet;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FleetTelemetryAPI.Services;

public class VehicleService : IVehicleService
{
    private readonly FleetContext _context;

    public VehicleService(FleetContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResultDto<VehicleOutputDto>> GetAllVehiclesAsync(PaginationQueryDto pagination)
    {
        var vehicleQuery = _context.Vehicles
            .Where(v => v.Status == VehicleStatus.Active || v.Status == VehicleStatus.Maintenance);
        var totalVehicles = await vehicleQuery.CountAsync();



        var vehicles = await vehicleQuery
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(vehicles => new VehicleOutputDto
            {
                Id = vehicles.Id,
                RegistrationNumber = vehicles.RegistrationNumber,
                Brand = vehicles.Brand,
                Model = vehicles.Model,
                Year = vehicles.Year,
                LoadCapacity = vehicles.LoadCapacity,
                Type = vehicles.Type,
                Status = vehicles.Status
            })
            .ToListAsync();

        return new PaginatedResultDto<VehicleOutputDto>
        {
            TotalCount = totalVehicles,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Data = vehicles
        };
    }

    public async Task<Result<VehicleOutputDto>> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _context.Vehicles
            .Where(v => v.Id == id)
            .Select(vehicles => new VehicleOutputDto
            {
                Id = vehicles.Id,
                RegistrationNumber = vehicles.RegistrationNumber,
                Brand = vehicles.Brand,
                Model = vehicles.Model,
                Year = vehicles.Year,
                LoadCapacity = vehicles.LoadCapacity,
                Type = vehicles.Type,
                Status = vehicles.Status
            })
            .FirstOrDefaultAsync();

        if (vehicle == null)
        {
            return Result<VehicleOutputDto>.Failure(ErrorType.NotFound, "Vehicle not found.");
        }

        return Result<VehicleOutputDto>.Success(vehicle);
    }

    public async Task<Result<VehicleOutputDto>> CreateVehicleAsync(VehicleInputDto vehicle)
    {
        var vehicleExists = await _context.Vehicles.AnyAsync(v => v.RegistrationNumber == vehicle.RegistrationNumber);

        if (vehicleExists)
        {
            return Result<VehicleOutputDto>.Failure(ErrorType.Conflict, "A vehicle with the same registration number already exists.");
        }

        var newVehicle = new Vehicle
        {
            RegistrationNumber = vehicle.RegistrationNumber,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            LoadCapacity = vehicle.LoadCapacity,
            Type = vehicle.Type,
            Status = VehicleStatus.Active

        };

        _context.Vehicles.Add(newVehicle);
        await _context.SaveChangesAsync();

        var outputVehicle = new VehicleOutputDto
        {
            Id = newVehicle.Id,
            RegistrationNumber = newVehicle.RegistrationNumber,
            Brand = newVehicle.Brand,
            Model = newVehicle.Model,
            Year = newVehicle.Year,
            LoadCapacity = newVehicle.LoadCapacity,
            Type = newVehicle.Type,
            Status = newVehicle.Status
        };

        return Result<VehicleOutputDto>.Success(outputVehicle);
    }

    public async Task<Result> UpdateVehicleAsync(int id, VehicleInputDto vehicle)
    {
        var vehicleToUpdate = await _context.Vehicles.FindAsync(id);

        if (vehicleToUpdate == null)
        {
            return Result.Failure(ErrorType.NotFound, "Vehicle not found.");
        }

        var vehicleDuplicated = await _context.Vehicles.AnyAsync(vtu => vtu.RegistrationNumber == vehicle.RegistrationNumber && vtu.Id != id);

        if (vehicleDuplicated)
        {
            return Result.Failure(ErrorType.Conflict, "A vehicle with the same registration number already exists.");
        }

        vehicleToUpdate.RegistrationNumber = vehicle.RegistrationNumber;
        vehicleToUpdate.Brand = vehicle.Brand;
        vehicleToUpdate.Model = vehicle.Model;
        vehicleToUpdate.Year = vehicle.Year;
        vehicleToUpdate.LoadCapacity = vehicle.LoadCapacity;
        vehicleToUpdate.Type = vehicle.Type;

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateVehicleStatusAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return Result.Failure(ErrorType.NotFound, "Vehicle not found.");
        }

        if (vehicle.Status == VehicleStatus.Active)
        {
            vehicle.Status = VehicleStatus.Maintenance;
        }
        else if (vehicle.Status == VehicleStatus.Maintenance)
        {
            vehicle.Status = VehicleStatus.Active;
        }
        else
        {
            return Result.Failure(ErrorType.Validation, "Vehicle status cannot be changed from Inactive.");
        }

        await _context.SaveChangesAsync();
        return Result.Success();

    }

    public async Task<Result> DeleteVehicleAsync(int id)
    {
        var vehicleToDelete = await _context.Vehicles.FindAsync(id);

        if (vehicleToDelete == null)
        {
            return Result.Failure(ErrorType.NotFound, "Vehicle not found.");
        }

        vehicleToDelete.Status = VehicleStatus.Inactive;

        await _context.SaveChangesAsync();
        return Result.Success();
    }

}
