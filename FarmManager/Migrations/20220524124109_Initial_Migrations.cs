using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManager.Migrations
{
    public partial class Initial_Migrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "farm-manager");

            migrationBuilder.CreateTable(
                name: "FarmEntity",
                schema: "farm-manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmEntity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmEntity",
                schema: "farm-manager");
        }
    }
}
