using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.Models;
using FleetTelemetryAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FleetTelemetryAPI.Services;

namespace FleetTelemetryAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class VehicleAssignmentsController : ControllerBase
{
    private readonly IVehicleAssignmentService _service;

    public VehicleAssignmentsController(IVehicleAssignmentService service)
    {
        _service = service;
    }

    // GET: api/VehicleAssignments
    [HttpGet]
    public async Task<ActionResult> GetAllVehicleAssignments([FromQuery] PaginationQueryDto pagination)
    {
        var vehicleAssignments = await _service.GetAllVehicleAssignmentsAsync(pagination);
        return Ok(vehicleAssignments);
    }

    // GET: api/VehicleAssignments/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetVehicleAssignmentById([FromRoute] int id)
    {
        var vehicleAssignment = await _service.GetVehicleAssignmentByIdAsync(id);

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
        var result = await _service.CreateVehicleAssignmentAsync(vehicleAssignment);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage.StartsWith("Bad Request"))
            {
                return BadRequest(result.ErrorMessage.Substring(13));
            }

            return Conflict(result.ErrorMessage.Substring(10));
        }

        return CreatedAtAction(nameof(GetVehicleAssignmentById), new { id = result.Data!.Id }, result.Data);
    }

    // PUT: api/VehicleAssignments/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVehicleAssignment([FromRoute] int id, [FromBody] VehicleAssignmentInputDto vehicleAssignment)
    {
        var result = await _service.UpdateVehicleAssignmentAsync(id, vehicleAssignment);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage.StartsWith("Not Found"))
            {
                return NotFound();
            }

            if (result.ErrorMessage.StartsWith("Bad Request"))
            {
                return BadRequest(result.ErrorMessage.Substring(13));
            }
            
            return Conflict(result.ErrorMessage.Substring(10));
        }

        return NoContent();
    }

    // POST: api/VehicleAssignments/5/complete
    [HttpPost("{id}/complete")]
    public async Task<ActionResult> ReturnVehicle([FromRoute] int id)
    {
        var result = await _service.ReturnVehicle(id);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage.StartsWith("Not Found"))
            {
                return NotFound();
            }

            return BadRequest(result.ErrorMessage.Substring(13));
        }

        return NoContent();
    }
}
