using FleetTelemetryAPI.Models.Identity;

namespace FleetTelemetryAPI.Data.Seeders;

public static class AuthDbSeeder
{
    public static void SeedAdminUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthContext>();

        if (context.Employees.Any())
        {
            return;
        }


        var adminUser = new Employee
        {
            Username = "admin",
            Email = "admin@fleet.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = RoleType.Admin
        };

        context.Employees.Add(adminUser);
        context.SaveChanges();
    }
}

