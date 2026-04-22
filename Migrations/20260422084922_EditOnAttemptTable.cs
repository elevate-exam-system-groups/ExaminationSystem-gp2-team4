using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examination_System.Migrations
{
    /// <inheritdoc />
    public partial class EditOnAttemptTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPassed",
                table: "Attempts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPassed",
                table: "Attempts");
        }
    }
}
