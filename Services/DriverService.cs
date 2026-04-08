using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Fleet;
using FleetTelemetryAPI.Models.Fleet;
using Microsoft.EntityFrameworkCore;

namespace FleetTelemetryAPI.Services;

public class DriverService : IDriverService
{
    private readonly FleetContext _context;

    public DriverService(FleetContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResultDto<DriverOutputDto>> GetAllDriversAsync(PaginationQueryDto pagination)
    {
        var driverQuery = _context.Drivers.Where(d => d.IsActive);
        var totalDrivers = await driverQuery.CountAsync();

        var drivers = await driverQuery
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(drivers => new DriverOutputDto
            {
                Id = drivers.Id,
                Name = drivers.Name,
                LicenseNumber = drivers.LicenseNumber,
                LicenseCategory = drivers.LicenseCategory,
                IsActive = drivers.IsActive
            })
            .ToListAsync();

        return new PaginatedResultDto<DriverOutputDto>
        {
            TotalCount = totalDrivers,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Data = drivers
        };
    }

    public async Task<Result<DriverOutputDto>> GetDriverByIdAsync(int id)
    {
        var driver = await _context.Drivers
            .Where(d => d.Id == id)
            .Select(drivers => new DriverOutputDto
            {
                Id = drivers.Id,
                Name = drivers.Name,
                LicenseNumber = drivers.LicenseNumber,
                LicenseCategory = drivers.LicenseCategory,
                IsActive = drivers.IsActive
            })
            .FirstOrDefaultAsync();

        if (driver == null)
        {
            return Result<DriverOutputDto>.Failure(ErrorType.NotFound, "Driver not found");
        }

        return Result<DriverOutputDto>.Success(driver);
    }

    public async Task<Result<DriverOutputDto>> CreateDriverAsync(DriverInputDto driver)
    {
        var driverExists = await _context.Drivers.AnyAsync(d => d.LicenseNumber == driver.LicenseNumber);

        if (driverExists)
        {
            return Result<DriverOutputDto>.Failure(ErrorType.Conflict, "A driver with the same license number already exists.");
        }

        var newDriver = new Driver
        {
            Name = driver.Name,
            LicenseNumber = driver.LicenseNumber,
            LicenseCategory = driver.LicenseCategory,
        };

        _context.Drivers.Add(newDriver);
        await _context.SaveChangesAsync();

        var outputDriver = new DriverOutputDto
        {
            Id = newDriver.Id,
            Name = newDriver.Name,
            LicenseNumber = newDriver.LicenseNumber,
            LicenseCategory = newDriver.LicenseCategory,
            IsActive = newDriver.IsActive
        };

        return Result<DriverOutputDto>.Success(outputDriver);
    }

    public async Task<Result> UpdateDriverAsync(int id, DriverInputDto driver)
    {
        var driverToUpdate = await _context.Drivers.FindAsync(id);

        if (driverToUpdate == null)
        {
            return Result.Failure(ErrorType.NotFound, "Driver not found");
        }

        var driverDuplicated = await _context.Drivers.AnyAsync(d => d.LicenseNumber == driver.LicenseNumber && d.Id != id);

        if (driverDuplicated)
        {
            return Result.Failure(ErrorType.Conflict, "A driver with the same license number already exists.");
        }

        driverToUpdate.Name = driver.Name;
        driverToUpdate.LicenseNumber = driver.LicenseNumber;
        driverToUpdate.LicenseCategory = driver.LicenseCategory;

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteDriverAsync(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);

        if (driver == null)
        {
            return Result.Failure(ErrorType.NotFound, "Driver not found");
        }

        driver.IsActive = false;
        await _context.SaveChangesAsync();
        return Result.Success();
    }


}
