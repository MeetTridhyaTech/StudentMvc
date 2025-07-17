using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCourseSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentCourseId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ParentCourseId",
                table: "Courses",
                column: "ParentCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Courses_ParentCourseId",
                table: "Courses",
                column: "ParentCourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Courses_ParentCourseId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ParentCourseId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ParentCourseId",
                table: "Courses");
        }
    }
}
