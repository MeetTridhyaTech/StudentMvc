using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Course",
                table: "Students",
                newName: "CourseNames");

            migrationBuilder.AddColumn<string>(
                name: "CourseIds",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseIds",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "CourseNames",
                table: "Students",
                newName: "Course");
        }
    }
}
