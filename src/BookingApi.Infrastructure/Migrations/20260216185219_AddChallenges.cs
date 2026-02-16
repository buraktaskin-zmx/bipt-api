using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    ChallengeId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ChallengeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Condition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RewardPoints = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.ChallengeId);
                });

            migrationBuilder.InsertData(
                table: "Challenges",
                columns: new[] { "ChallengeId", "ChallengeName", "Condition", "IsActive", "Priority", "RewardPoints" },
                values: new object[,]
                {
                    { "C-01", "Günlük Mesajcı", "messages_today >= 20", true, 4, 50 },
                    { "C-02", "Etkileşim Ustası", "reactions_today >= 15", true, 3, 80 },
                    { "C-03", "Grup Lideri", "unique_groups_today >= 3", true, 2, 120 },
                    { "C-04", "Haftalık Aktif", "messages_7d >= 150", true, 1, 200 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Challenges");
        }
    }
}
