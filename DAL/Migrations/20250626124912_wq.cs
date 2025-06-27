using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class wq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseVideos_Users_TeacherId",
                table: "CourseVideos");

            migrationBuilder.DropIndex(
                name: "IX_CourseVideos_TeacherId",
                table: "CourseVideos");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "CourseVideos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "CourseVideos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CourseVideos_TeacherId",
                table: "CourseVideos",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseVideos_Users_TeacherId",
                table: "CourseVideos",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
