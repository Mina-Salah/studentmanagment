using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addcourcestote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "CourseVideos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseVideos_CourseId",
                table: "CourseVideos",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseVideos_Courses_CourseId",
                table: "CourseVideos",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseVideos_Courses_CourseId",
                table: "CourseVideos");

            migrationBuilder.DropIndex(
                name: "IX_CourseVideos_CourseId",
                table: "CourseVideos");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseVideos");
        }
    }
}
