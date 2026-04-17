using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examination_System.Migrations
{
    /// <inheritdoc />
    public partial class FixEntitiesAndMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Quizzes",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Attempts",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_UserId",
                table: "Attempts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attempts_Users_UserId",
                table: "Attempts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attempts_Users_UserId",
                table: "Attempts");

            migrationBuilder.DropIndex(
                name: "IX_Attempts_UserId",
                table: "Attempts");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Quizzes",
                newName: "status");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Attempts",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
