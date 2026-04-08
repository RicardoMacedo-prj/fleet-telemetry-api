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
public class VehiclesController : ApiControllerBase
{
    private readonly IVehicleService _service;

    public VehiclesController(IVehicleService service)
    {
        _service = service;
    }

    // GET: api/Vehicles
    [HttpGet]
    public async Task<ActionResult> GetAllVehicles([FromQuery] PaginationQueryDto pagination)
    {
        var vehicles = await _service.GetAllVehiclesAsync(pagination);
        return Ok(vehicles);

    }


    // GET: api/Vehicles/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetVehicleById([FromRoute] int id)
    {
        var result = await _service.GetVehicleByIdAsync(id);

        if (result.IsSuccess)
        {
            return HandleFailure(result);
        }

        return Ok(result.Data);
    }

    // POST: api/Vehicles
    [HttpPost]
    public async Task<ActionResult> CreateVehicle([FromBody] VehicleInputDto vehicle)
    {
        var result = await _service.CreateVehicleAsync(vehicle);

        if(!result.IsSuccess)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(nameof(GetVehicleById), new { id = result.Data!.Id }, result.Data);
    }

    // POST: api/Vehicles/5/status
    [HttpPost("{id}/status")]
    public async Task<ActionResult> UpdateVehicleStatus([FromRoute] int id)
    {
        var result = await _service.UpdateVehicleStatusAsync(id);

        if (!result.IsSuccess)
        {
            HandleFailure(result);
        }
            
        return NoContent();
    }


    // PUT: api/Vehicles/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVehicle([FromRoute] int id,[FromBody]  VehicleInputDto vehicle)
    {
        var result = await _service.UpdateVehicleAsync(id, vehicle);

        if (!result.IsSuccess)
        {
            return HandleFailure(result);
        }

        return NoContent();

    }

    // DELETE: api/Vehicles/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle([FromRoute] int id)
    {
        var result = await _service.DeleteVehicleAsync(id);

        if (!result.IsSuccess)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}
