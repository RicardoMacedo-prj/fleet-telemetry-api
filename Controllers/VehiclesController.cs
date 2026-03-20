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
public class VehiclesController : ControllerBase
{
    private readonly FleetContext _context;

    public VehiclesController(FleetContext context)
    {
        _context = context;
    }

    // GET: api/Vehicles
    [HttpGet]
    public async Task<ActionResult> GetAllVehicles([FromQuery] PaginationQueryDto pagination)
    {
        var vehicleQuery = _context.Vehicles
            .Where(v => v.Status == VehicleStatus.Active || v.Status == VehicleStatus.Maintenance);
        var totalVehicles = await vehicleQuery.CountAsync();

        
        
        var vehicles = await vehicleQuery
            .Skip((pagination.PageNumber -1) * pagination.PageSize)
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

        var result = new PaginatedResultDto<VehicleOutputDto>
        {
            TotalCount = totalVehicles,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Data = vehicles
        };

        return Ok(result);
    }


    // GET: api/Vehicles/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetVehicleById([FromRoute] int id)
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
            return NotFound();
        }

        return Ok(vehicle);
    }

    // POST: api/Vehicles
    [HttpPost]
    public async Task<ActionResult> CreateVehicle([FromBody] VehicleInputDto vehicle)
    {
        var vehicleExists = await _context.Vehicles.AnyAsync(v => v.RegistrationNumber == vehicle.RegistrationNumber);

        if (vehicleExists)
        {
            return Conflict("A vehicle with the same registration number already exists.");
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

        return CreatedAtAction(nameof(GetVehicleById), new { id = newVehicle.Id }, outputVehicle);
    }

    // POST: api/Vehicles/5/status
    [HttpPost("{id}/status")]
    public async Task<ActionResult> UpdateVehicleStatus([FromRoute] int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
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
            return BadRequest("Vehicle status cannot be changed from Inactive.");
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }


    // PUT: api/Vehicles/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVehicle([FromRoute] int id,[FromBody]  VehicleInputDto vehicle)
    { 
        var vehicleToUpdate = await _context.Vehicles.FindAsync(id);

        if (vehicleToUpdate == null)
        {
            return NotFound();
        }

        var vehicleDuplicated = await _context.Vehicles.AnyAsync(vtu => vtu.RegistrationNumber == vehicle.RegistrationNumber && vtu.Id != id);

        if (vehicleDuplicated)
        {
            return Conflict("A vehicle with the same registration number already exists.");
        }

        vehicleToUpdate.RegistrationNumber = vehicle.RegistrationNumber;
        vehicleToUpdate.Brand = vehicle.Brand;
        vehicleToUpdate.Model = vehicle.Model;
        vehicleToUpdate.Year = vehicle.Year;
        vehicleToUpdate.LoadCapacity = vehicle.LoadCapacity;
        vehicleToUpdate.Type = vehicle.Type;

        await _context.SaveChangesAsync();
        return NoContent();

    }

    // DELETE: api/Vehicles/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle([FromRoute] int id)
    {
        var vehicleToDelete = await _context.Vehicles.FindAsync(id);

        if (vehicleToDelete == null)
        {
            return NotFound();
        }

        vehicleToDelete.Status = VehicleStatus.Inactive;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
