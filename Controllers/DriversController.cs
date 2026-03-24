using System.Data;
using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.Models;
using FleetTelemetryAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FleetTelemetryAPI.Services;
using FleetTelemetryAPI.DTOs.Fleet;

namespace FleetTelemetryAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class DriversController : ControllerBase
{
    private readonly IDriverService _service;

    public DriversController(IDriverService service)
    {
        _service = service;
    }

    // GET: api/Drivers
    [HttpGet]
    public async Task<ActionResult> GetAllDrivers([FromQuery] PaginationQueryDto pagination)
    {
        var drivers = await _service.GetAllDriversAsync(pagination);
        return Ok(drivers);
    }

    // GET: api/Drivers/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetDriverById([FromRoute] int id)
    {
        var driver = await _service.GetDriverByIdAsync(id);

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
        var result = await _service.CreateDriverAsync(driver);

        if (!result.IsSuccess)
        {
            return Conflict(result.ErrorMessage.Substring(10));
        }

        return CreatedAtAction(nameof(GetDriverById), new { id = result.Data!.Id }, result.Data);
    }

    // PUT: api/Drivers/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateDriver([FromRoute] int id, [FromBody] DriverInputDto driver)
    {
        var result = await _service.UpdateDriverAsync(id, driver);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage.StartsWith("Not Found"))
            {
                return NotFound();
            }

            return Conflict(result.ErrorMessage.Substring(10));
        }

        return NoContent();
    }

    // DELETE: api/Drivers/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDriver([FromRoute] int id)
    {
        var result =await _service.DeleteDriverAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
        
    }
}
