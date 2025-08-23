using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigFiveAssessmentApi.Migrations
{
    /// <inheritdoc />
    public partial class BigFiveAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CandidateName = table.Column<string>(type: "TEXT", nullable: false),
                    CandidateEmail = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResponsesJson = table.Column<string>(type: "TEXT", nullable: false),
                    ScoresJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submissions");
        }
    }
}
