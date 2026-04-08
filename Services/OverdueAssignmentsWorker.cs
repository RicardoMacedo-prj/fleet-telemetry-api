using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.Models.Fleet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FleetTelemetryAPI.Services;

public class OverdueAssignmentsWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public OverdueAssignmentsWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FleetContext>();

                await context.VehicleAssignments
                    .Where(va => va.ReturnDate == null 
                        && va.ExpectedReturnDate < DateTime.UtcNow 
                        && va.Status == AssignmentStatus.Active)
                    .ExecuteUpdateAsync(s => s.SetProperty(va => va.Status, AssignmentStatus.Overdue), stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
    
    
}
