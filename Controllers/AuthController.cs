using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FleetTelemetryAPI.Common;
using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Identity;
using FleetTelemetryAPI.Models.Identity;
using FleetTelemetryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FleetTelemetryAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto register)
    {
        var result = await _service.RegisterAsync(register);

        if (!result.IsSuccess)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var result = await _service.LoginAsync(login);

        if (!result.IsSuccess)
        {
            return HandleFailure(result);
        }

        return Ok(result.Data);
    }
}
