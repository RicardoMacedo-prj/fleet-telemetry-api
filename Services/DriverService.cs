using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.Models;
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

    public async Task<DriverOutputDto?> GetDriverByIdAsync(int id)
    {
        return await _context.Drivers
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
    }

    public async Task<(bool IsSuccess, string ErrorMessage, DriverOutputDto? Data)> CreateDriverAsync(DriverInputDto driver)
    {
        var driverExists = await _context.Drivers.AnyAsync(d => d.LicenseNumber == driver.LicenseNumber);

        if (driverExists)
        {
            return (false, "Conflict: A driver with the same license number already exists.", null);
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

        return (true, string.Empty, outputDriver);
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> UpdateDriverAsync(int id, DriverInputDto driver)
    {
        var driverToUpdate = await _context.Drivers.FindAsync(id);

        if (driverToUpdate == null)
        {
            return (false, "Not Found");
        }

        var driverDuplicated = await _context.Drivers.AnyAsync(d => d.LicenseNumber == driver.LicenseNumber && d.Id != id);

        if (driverDuplicated)
        {
            return (false, "Conflict: A driver with the same license number already exists.");
        }

        driverToUpdate.Name = driver.Name;
        driverToUpdate.LicenseNumber = driver.LicenseNumber;
        driverToUpdate.LicenseCategory = driver.LicenseCategory;

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<bool> DeleteDriverAsync(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);

        if (driver == null)
        {
            return false;
        }

        driver.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }


}
