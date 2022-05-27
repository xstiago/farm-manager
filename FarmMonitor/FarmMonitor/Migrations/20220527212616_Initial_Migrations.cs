using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmMonitor.Migrations
{
    public partial class Initial_Migrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "farm-monitor");

            migrationBuilder.CreateTable(
                name: "DeviceEntity",
                schema: "farm-monitor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryEntity",
                schema: "farm-monitor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Temperature = table.Column<float>(type: "real", nullable: false),
                    Humidity = table.Column<float>(type: "real", nullable: false),
                    MeasurementDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelemetryEntity_DeviceEntity_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "farm-monitor",
                        principalTable: "DeviceEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryEntity_DeviceId",
                schema: "farm-monitor",
                table: "TelemetryEntity",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemetryEntity",
                schema: "farm-monitor");

            migrationBuilder.DropTable(
                name: "DeviceEntity",
                schema: "farm-monitor");
        }
    }
}
