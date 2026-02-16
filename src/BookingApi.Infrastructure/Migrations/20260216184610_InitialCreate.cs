using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    BadgeId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    BadgeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Condition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.BadgeId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "BadgeAwards",
                columns: table => new
                {
                    AwardId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    BadgeId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BadgeAwards", x => x.AwardId);
                    table.ForeignKey(
                        name: "FK_BadgeAwards_Badges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "Badges",
                        principalColumn: "BadgeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BadgeAwards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointsLedgers",
                columns: table => new
                {
                    LedgerId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PointsDelta = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SourceRef = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsLedgers", x => x.LedgerId);
                    table.ForeignKey(
                        name: "FK_PointsLedgers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "BadgeId", "BadgeName", "Condition", "Level" },
                values: new object[,]
                {
                    { "B1", "🥉 Bronz Sosyal", "total_points >= 200", 1 },
                    { "B2", "🥈 Gümüş Sosyal", "total_points >= 600", 2 },
                    { "B3", "🥇 Altın Sosyal", "total_points >= 1000", 3 }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "GroupId", "GroupName" },
                values: new object[,]
                {
                    { "G1", "Tech Sohbet" },
                    { "G2", "Futbol Muhabbet" },
                    { "G3", "Oyun Ekibi" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "City", "Name" },
                values: new object[,]
                {
                    { "U1", "Istanbul", "Ayşe" },
                    { "U2", "Ankara", "Ali" },
                    { "U3", "Izmir", "Deniz" },
                    { "U4", "Bursa", "Mert" },
                    { "U5", "Antalya", "Ece" }
                });

            migrationBuilder.InsertData(
                table: "BadgeAwards",
                columns: new[] { "AwardId", "BadgeId", "UserId" },
                values: new object[,]
                {
                    { "BA-1", "B1", "U1" },
                    { "BA-2", "B1", "U2" },
                    { "BA-3", "B1", "U3" },
                    { "BA-4", "B1", "U4" }
                });

            migrationBuilder.InsertData(
                table: "PointsLedgers",
                columns: new[] { "LedgerId", "PointsDelta", "Source", "SourceRef", "UserId" },
                values: new object[,]
                {
                    { "L-200", 200, "CHALLENGE", "A-100", "U1" },
                    { "L-201", 200, "CHALLENGE", "A-101", "U2" },
                    { "L-202", 200, "CHALLENGE", "A-102", "U3" },
                    { "L-203", 200, "CHALLENGE", "A-103", "U4" },
                    { "L-204", 50, "CHALLENGE", "A-104", "U5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAwards_BadgeId",
                table: "BadgeAwards",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAwards_UserId",
                table: "BadgeAwards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsLedgers_UserId",
                table: "PointsLedgers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BadgeAwards");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "PointsLedgers");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
