using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.Models;
using FleetTelemetryAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FleetTelemetryAPI.Services;
using FleetTelemetryAPI.DTOs.Fleet;
using FleetTelemetryAPI.Common;

namespace FleetTelemetryAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class VehicleAssignmentsController : ApiControllerBase
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
        var result = await _service.GetVehicleAssignmentByIdAsync(id);

        if (!result.IsSuccess)
        {
            return HandleFailure(result);
        }

        return Ok(result.Data);
    }

    // POST: api/VehicleAssignments
    [HttpPost]
    public async Task<ActionResult> CreateVehicleAssignment([FromBody] VehicleAssignmentInputDto vehicleAssignment)
    {
        var result = await _service.CreateVehicleAssignmentAsync(vehicleAssignment);

        if (!result.IsSuccess)
        {
            return HandleFailure(result);
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
            return HandleFailure(result);
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
            return HandleFailure(result);
        }

        return NoContent();
    }
}
