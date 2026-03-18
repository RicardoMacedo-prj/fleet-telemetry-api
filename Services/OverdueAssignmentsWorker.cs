using FleetTelemetryAPI.Data;
using FleetTelemetryAPI.Models;
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

                var overdueAssignments = await context.VehicleAssignments
                    .Where(va => va.ReturnDate == null 
                        && va.ExpectedReturnDate < DateTime.UtcNow 
                        && va.Status == AssignmentStatus.Active)
                    .ToListAsync(stoppingToken);

                if (overdueAssignments.Any())
                {
                    foreach (var assignment in overdueAssignments)
                    {
                        assignment.Status = AssignmentStatus.Overdue;
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
    
    
}
