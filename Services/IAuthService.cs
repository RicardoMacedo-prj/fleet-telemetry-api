using FleetTelemetryAPI.DTOs;
using FleetTelemetryAPI.DTOs.Identity;

namespace FleetTelemetryAPI.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterDto register);
    Task<Result<string>> LoginAsync(LoginDto login);
}
