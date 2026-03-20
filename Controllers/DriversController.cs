using System.Data;
using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.Models;
using FleetTelemetryAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FleetTelemetryAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class DriversController : ControllerBase
{
    private readonly FleetContext _context;

    public DriversController(FleetContext context)
    {
        _context = context;
    }

    // GET: api/Drivers
    [HttpGet]
    public async Task<ActionResult> GetAllDrivers([FromQuery] PaginationQueryDto pagination)
    {
        var driverQuery = _context.Drivers.Where(d => d.IsActive);
        var totalDrivers = await driverQuery.CountAsync();

        var drivers = await driverQuery
            .Skip((pagination.PageNumber -1) * pagination.PageSize)
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

        var result = new PaginatedResultDto<DriverOutputDto>
        {
            TotalCount = totalDrivers,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Data = drivers
        };

        return Ok(result);
    }

    // GET: api/Drivers/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetDriverById([FromRoute] int id)
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
            return NotFound();
        }

        return Ok(driver);
    }

    // POST: api/Drivers
    [HttpPost]
    public async Task<ActionResult> CreateDriver([FromBody] DriverInputDto driver)
    {
        var driverExists = await _context.Drivers.AnyAsync(d => d.LicenseNumber == driver.LicenseNumber);

        if (driverExists)
        {
            return Conflict("A driver with the same license number already exists.");
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

        return CreatedAtAction(nameof(GetDriverById), new { id = newDriver.Id }, outputDriver);
    }

    // PUT: api/Drivers/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateDriver([FromRoute] int id, [FromBody] DriverInputDto driver)
    {
        var driverToUpdate = await _context.Drivers.FindAsync(id);

        if (driverToUpdate == null)
        {
            return NotFound();
        }

        var driverDuplicated = await _context.Drivers.AnyAsync(d => d.LicenseNumber == driver.LicenseNumber && d.Id != id);

        if (driverDuplicated)
        {
            return Conflict("A driver with the same license number already exists.");
        }

        driverToUpdate.Name = driver.Name;
        driverToUpdate.LicenseNumber = driver.LicenseNumber;
        driverToUpdate.LicenseCategory = driver.LicenseCategory;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Drivers/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDriver([FromRoute] int id)
    {
        var driver = await _context.Drivers.FindAsync(id);

        if (driver == null)
        {
            return NotFound();
        }

        driver.IsActive = false;
        await _context.SaveChangesAsync();
        return NoContent();
        
    }
}
