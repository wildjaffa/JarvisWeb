using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JarvisWeb.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddIsInOffice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastSeenDailySummaryId",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSeenDailySummaryId",
                table: "Users");
        }
    }
}
