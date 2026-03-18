 using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetTelemetryAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddExpectedReturnDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedReturnDate",
                table: "VehicleAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "VehicleAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedReturnDate",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VehicleAssignments");
        }
    }
}
