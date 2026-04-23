using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examination_System.Migrations
{
    /// <inheritdoc />
    public partial class OtherFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "Questions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_AttemptId",
                table: "Answers",
                column: "AttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Attempts_AttemptId",
                table: "Answers",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Attempts_AttemptId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_AttemptId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "Questions");
        }
    }
}
