namespace FleetTelemetryAPI.DTOs;

public class PaginatedResultDto<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public required IEnumerable<T> Data { get; set; }
}
