using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courses.Migrations
{
    /// <inheritdoc />
    public partial class initial6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingSlotsCount",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingSlotsCount",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
