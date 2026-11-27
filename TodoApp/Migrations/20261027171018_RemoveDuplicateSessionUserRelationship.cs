using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateSessionUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_UserId1",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_UserId1",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Sessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Sessions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId1",
                table: "Sessions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_UserId1",
                table: "Sessions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
