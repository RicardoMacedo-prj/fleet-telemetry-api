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
public class VehicleAssignmentsController : ControllerBase
{
    private readonly FleetContext _context;

    public VehicleAssignmentsController(FleetContext context)
    {
        _context = context;
    }

    // GET: api/VehicleAssignments
    [HttpGet]
    public async Task<ActionResult> GetAllVehicleAssignments()
    {
        var vehicleAssignments = await _context.VehicleAssignments
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
        return Ok(vehicleAssignments);
    }

    // GET: api/VehicleAssignments/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetVehicleAssignmentById([FromRoute] int id)
    {
        var vehicleAssignment = await _context.VehicleAssignments
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

        if (vehicleAssignment == null)
        {
            return NotFound();
        }
        return Ok(vehicleAssignment);
    }

    // POST: api/VehicleAssignments
    [HttpPost]
    public async Task<ActionResult> CreateVehicleAssignment([FromBody] VehicleAssignmentInputDto vehicleAssignment)
    {
        DateTime AssignmentDate = DateTime.UtcNow;

        if (vehicleAssignment.ExpectedReturnDate != null && vehicleAssignment.ExpectedReturnDate < AssignmentDate)
        {
            return BadRequest("Invalid expected return date.");
        }

        var driver = await _context.Drivers.FindAsync(vehicleAssignment.DriverId);
        var vehicle = await _context.Vehicles.FindAsync(vehicleAssignment.VehicleId);

        if (driver == null || vehicle == null)
        {
            return BadRequest("Driver or Vehicle does not exist.");
        }

        if ((int)driver.LicenseCategory != (int)vehicle.Type)
        {

            return BadRequest("Driver's license category does not match vehicle type.");
        }

        var driverAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.DriverId == vehicleAssignment.DriverId && va.ReturnDate == null);

        var vehicleAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.VehicleId == vehicleAssignment.VehicleId && va.ReturnDate == null);

        if (driverAlreadyAssigned)
        {
            return Conflict("Driver is already assigned to another vehicle.");
        }
        if (vehicleAlreadyAssigned)
        {
            return Conflict("Vehicle is already assigned to another driver.");
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

        return CreatedAtAction(nameof(GetVehicleAssignmentById), new { id = newAssignment.Id }, outputVehicleAssignment);

    }

    // PUT: api/VehicleAssignments/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVehicleAssignment([FromRoute] int id, [FromBody] VehicleAssignmentInputDto vehicleAssignment)
    {
        var AssignmentToUpdate = await _context.VehicleAssignments.FindAsync(id);

        if (AssignmentToUpdate == null)
        {
            return NotFound();
        }

        if (vehicleAssignment.ExpectedReturnDate != null && 
            vehicleAssignment.ExpectedReturnDate < AssignmentToUpdate.AssignmentDate)
        {
            return BadRequest("Invalid expected return date.");
        }

        var driver = await _context.Drivers.FindAsync(vehicleAssignment.DriverId);
        var vehicle = await _context.Vehicles.FindAsync(vehicleAssignment.VehicleId);

        if (driver == null|| vehicle == null)
        {
            return BadRequest("Driver or Vehicle does not exist.");
        }

        if ((int)driver.LicenseCategory != (int)vehicle.Type) {

            return BadRequest("Driver's license category does not match vehicle type.");
        }

        var driverAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.DriverId == vehicleAssignment.DriverId && va.Id != id && va.ReturnDate == null);
        
        var vehicleAlreadyAssigned = await _context.VehicleAssignments
            .AnyAsync(va => va.VehicleId == vehicleAssignment.VehicleId && va.Id != id && va.ReturnDate == null);
        
        if (driverAlreadyAssigned)
        {
            return Conflict("Driver is already assigned to another vehicle.");
        }
        if (vehicleAlreadyAssigned)
        {
            return Conflict("Vehicle is already assigned to another driver.");
        } 

        AssignmentToUpdate.DriverId = vehicleAssignment.DriverId;
        AssignmentToUpdate.VehicleId = vehicleAssignment.VehicleId;
        AssignmentToUpdate.ExpectedReturnDate = vehicleAssignment.ExpectedReturnDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST: api/VehicleAssignments/5/complete
    [HttpPost("{id}/complete")]
    public async Task<ActionResult> ReturnVehicle([FromRoute] int id)
    {
        var assignment = await _context.VehicleAssignments.FindAsync(id);

        if (assignment == null)
        {
            return NotFound();
        }

        if (assignment.Status == AssignmentStatus.Completed)
        {
            return Conflict("Assignment is already Completed.");
        }

        assignment.ReturnDate = DateTime.UtcNow;
        assignment.Status = AssignmentStatus.Completed;

        await _context.SaveChangesAsync();
        return NoContent();


    }
}
