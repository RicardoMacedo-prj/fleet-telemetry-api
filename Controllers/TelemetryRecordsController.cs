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
public class TelemetryRecordsController : ControllerBase
{
    private readonly ITelemetryRecordService _service;

    public TelemetryRecordsController(ITelemetryRecordService service)
    {
        _service = service;
    }


    // GET: api/TelemetryRecords/VehicleId
    [HttpGet("{VehicleId}")]
    public async Task<ActionResult> GetTelemetryRecordById([FromRoute] int vehicleId, [FromQuery] PaginationQueryDto pagination)
    {
        var records = await _service.GetTelemetryRecordByIdAsync(vehicleId, pagination);

        if (records.TotalCount == 0)
        {
            return NotFound();
        } 

        return Ok(records);
    }

    // POST: api/TelemetryRecords
    [HttpPost]
    public async Task<ActionResult> CreateTelemetryRecord([FromBody] TelemetryRecordInputDto telemetryRecord)
    {

        var result = await _service.CreateTelemetryRecordAsync(telemetryRecord);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage.Substring(13));
        }

        return NoContent();
    }
}
