using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManager.Migrations
{
    public partial class Added_Devices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceEntity",
                schema: "farm-manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FarmId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceEntity_FarmEntity_FarmId",
                        column: x => x.FarmId,
                        principalSchema: "farm-manager",
                        principalTable: "FarmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceEntity_FarmId",
                schema: "farm-manager",
                table: "DeviceEntity",
                column: "FarmId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceEntity",
                schema: "farm-manager");
        }
    }
}
