using FleetTelemetryAPI.Models.Fleet;
using FleetTelemetryAPI.Models.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace FleetTelemetryAPI.Data;
public class FleetContext : DbContext
{
    public FleetContext(DbContextOptions<FleetContext> options) : base(options) { }

    public DbSet<Driver> Drivers { get; set; } = null!;
    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<VehicleAssignment> VehicleAssignments { get; set; } = null!;
    public DbSet<TelemetryRecord> TelemetryRecords { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Driver>()
            .HasIndex(d => d.LicenseNumber)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.RegistrationNumber)
            .IsUnique();

        modelBuilder.Entity<VehicleAssignment>()
            .HasOne(va => va.Driver)
            .WithMany(d => d.VehicleAssignments)
            .HasForeignKey(va => va.DriverId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<VehicleAssignment>()
            .HasOne(va => va.Vehicle)
            .WithMany(v => v.VehicleAssignments)
            .HasForeignKey(va => va.VehicleId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TelemetryRecord>()
            .HasOne(tr => tr.Vehicle)
            .WithMany(v => v.TelemetryRecords)
            .HasForeignKey(tr => tr.VehicleId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TelemetryRecord>()
            .Property(tr => tr.Longitude)
            .HasPrecision(9, 6);

        modelBuilder.Entity<TelemetryRecord>()
            .Property(tr => tr.Latitude)
            .HasPrecision(9, 6);


    }

}
