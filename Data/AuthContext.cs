using FleetTelemetryAPI.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetTelemetryAPI.Data;

public class AuthContext: DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options): base(options) { }

    public DbSet<Employee> Employees { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        base.OnModelCreating(modelbuilder);

        modelbuilder.Entity<Employee>()
            .ToTable("Employees", "Auth");
        
    }
}
