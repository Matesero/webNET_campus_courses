using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courses.Migrations
{
    /// <inheritdoc />
    public partial class initial8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Annotation",
                table: "Courses",
                newName: "Annotations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Annotations",
                table: "Courses",
                newName: "Annotation");
        }
    }
}
