using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courses.Migrations
{
    /// <inheritdoc />
    public partial class initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isMain",
                table: "Teachers",
                newName: "IsMain");

            migrationBuilder.RenameColumn(
                name: "RemainingStoltsCount",
                table: "Courses",
                newName: "RemainingSlotsCount");

            migrationBuilder.AddColumn<string>(
                name: "Annotation",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annotation",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "IsMain",
                table: "Teachers",
                newName: "isMain");

            migrationBuilder.RenameColumn(
                name: "RemainingSlotsCount",
                table: "Courses",
                newName: "RemainingStoltsCount");
        }
    }
}
