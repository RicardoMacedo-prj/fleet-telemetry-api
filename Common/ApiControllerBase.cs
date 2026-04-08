using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetTelemetryAPI.Common;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult HandleFailure<T>(Result<T> result)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.Validation => BadRequest(result.ErrorMessage),
            ErrorType.Conflict => Conflict(result.ErrorMessage),
            ErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
            ErrorType.Forbidden => Forbid(result.ErrorMessage),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };
    }

    protected ActionResult HandleFailure(Result result)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.Validation => BadRequest(result.ErrorMessage),
            ErrorType.Conflict => Conflict(result.ErrorMessage),
            ErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
            ErrorType.Forbidden => Forbid(result.ErrorMessage),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };
    }
}
