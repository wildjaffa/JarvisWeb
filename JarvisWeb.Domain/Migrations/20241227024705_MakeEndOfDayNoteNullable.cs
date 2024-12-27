using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JarvisWeb.Domain.Migrations
{
    /// <inheritdoc />
    public partial class MakeEndOfDayNoteNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySummaries_EndOfDayNotes_EndOfDayNoteId",
                table: "DailySummaries");

            migrationBuilder.AlterColumn<Guid>(
                name: "EndOfDayNoteId",
                table: "DailySummaries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_DailySummaries_EndOfDayNotes_EndOfDayNoteId",
                table: "DailySummaries",
                column: "EndOfDayNoteId",
                principalTable: "EndOfDayNotes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySummaries_EndOfDayNotes_EndOfDayNoteId",
                table: "DailySummaries");

            migrationBuilder.AlterColumn<Guid>(
                name: "EndOfDayNoteId",
                table: "DailySummaries",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DailySummaries_EndOfDayNotes_EndOfDayNoteId",
                table: "DailySummaries",
                column: "EndOfDayNoteId",
                principalTable: "EndOfDayNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
