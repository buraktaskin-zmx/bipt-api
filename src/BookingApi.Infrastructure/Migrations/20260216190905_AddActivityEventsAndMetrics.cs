using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityEventsAndMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityEvents",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Messages = table.Column<int>(type: "integer", nullable: false),
                    Reactions = table.Column<int>(type: "integer", nullable: false),
                    UniqueGroups = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEvents", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_ActivityEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStates",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    AsOfDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MessagesToday = table.Column<int>(type: "integer", nullable: false),
                    ReactionsToday = table.Column<int>(type: "integer", nullable: false),
                    UniqueGroupsToday = table.Column<int>(type: "integer", nullable: false),
                    Messages7d = table.Column<int>(type: "integer", nullable: false),
                    Reactions7d = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStates", x => new { x.UserId, x.AsOfDate });
                    table.ForeignKey(
                        name: "FK_UserStates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ActivityEvents",
                columns: new[] { "EventId", "Date", "Messages", "Reactions", "UniqueGroups", "UserId" },
                values: new object[,]
                {
                    { "E-1", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), 13, 7, 1, "U1" },
                    { "E-2", new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc), 33, 5, 3, "U1" },
                    { "E-3", new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), 12, 23, 3, "U1" },
                    { "E-4", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), 27, 25, 1, "U1" },
                    { "E-5", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), 19, 8, 1, "U1" },
                    { "E-6", new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), 25, 19, 1, "U1" },
                    { "E-7", new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), 20, 21, 3, "U1" }
                });

            migrationBuilder.InsertData(
                table: "UserStates",
                columns: new[] { "AsOfDate", "UserId", "Messages7d", "MessagesToday", "Reactions7d", "ReactionsToday", "UniqueGroupsToday" },
                values: new object[,]
                {
                    { new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "U1", 169, 22, 108, 21, 3 },
                    { new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "U2", 150, 26, 109, 8, 1 },
                    { new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "U3", 185, 23, 88, 16, 2 },
                    { new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "U4", 218, 38, 129, 13, 1 },
                    { new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "U5", 104, 29, 70, 11, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEvents_UserId",
                table: "ActivityEvents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityEvents");

            migrationBuilder.DropTable(
                name: "UserStates");
        }
    }
}
