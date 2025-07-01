using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherRelationToCourseVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CourseVideos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "CourseVideos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseVideos_TeacherId",
                table: "CourseVideos",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseVideos_Teachers_TeacherId",
                table: "CourseVideos",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseVideos_Teachers_TeacherId",
                table: "CourseVideos");

            migrationBuilder.DropIndex(
                name: "IX_CourseVideos_TeacherId",
                table: "CourseVideos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CourseVideos");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "CourseVideos");
        }
    }
}
