using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace News.Migrations
{
    /// <inheritdoc />
    public partial class markerlogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Waypoints",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ImageFileId",
                table: "Waypoints",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Waypoints_ImageFileId",
                table: "Waypoints",
                column: "ImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Waypoints_Images_ImageFileId",
                table: "Waypoints",
                column: "ImageFileId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waypoints_Images_ImageFileId",
                table: "Waypoints");

            migrationBuilder.DropIndex(
                name: "IX_Waypoints_ImageFileId",
                table: "Waypoints");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Waypoints");

            migrationBuilder.DropColumn(
                name: "ImageFileId",
                table: "Waypoints");
        }
    }
}
