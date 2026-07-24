using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovesPresetsAndChangeCoordinatesStructureWithEpsg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "location_preset");

            migrationBuilder.DropTable(
                name: "requirement_preset");

            migrationBuilder.RenameColumn(
                name: "Coordinates_Longitude",
                table: "event_location",
                newName: "y");

            migrationBuilder.RenameColumn(
                name: "Coordinates_Latitude",
                table: "event_location",
                newName: "x");

            migrationBuilder.AddColumn<int>(
                name: "epsg",
                table: "event_location",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "epsg",
                table: "event_location");

            migrationBuilder.RenameColumn(
                name: "y",
                table: "event_location",
                newName: "Coordinates_Longitude");

            migrationBuilder.RenameColumn(
                name: "x",
                table: "event_location",
                newName: "Coordinates_Latitude");

            migrationBuilder.CreateTable(
                name: "location_preset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Coordinates_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Coordinates_Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_preset", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "requirement_preset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirement_preset", x => x.Id);
                });
        }
    }
}
