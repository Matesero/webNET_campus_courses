using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courses.Migrations
{
    /// <inheritdoc />
    public partial class initial14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationDate",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NotificationDate",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
