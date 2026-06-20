using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosVivos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "venues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    VenueId = table.Column<int>(type: "integer", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    StartAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TicketPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_events_venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_events_Status",
                table: "events",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_events_Type",
                table: "events",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_events_VenueId_StartAtUtc_EndAtUtc",
                table: "events",
                columns: new[] { "VenueId", "StartAtUtc", "EndAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "venues");
        }
    }
}
