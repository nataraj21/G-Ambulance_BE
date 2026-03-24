using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceOfficerWithUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Officers_OfficerId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_OfficerLocations_Officers_OfficerId",
                table: "OfficerLocations");

            migrationBuilder.DropTable(
                name: "Officers");

            migrationBuilder.RenameColumn(
                name: "OfficerId",
                table: "OfficerLocations",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OfficerLocations_OfficerId",
                table: "OfficerLocations",
                newName: "IX_OfficerLocations_UserId");

            migrationBuilder.RenameColumn(
                name: "OfficerId",
                table: "Alerts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Alerts_OfficerId",
                table: "Alerts",
                newName: "IX_Alerts_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Users_UserId",
                table: "Alerts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfficerLocations_Users_UserId",
                table: "OfficerLocations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Users_UserId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_OfficerLocations_Users_UserId",
                table: "OfficerLocations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OfficerLocations",
                newName: "OfficerId");

            migrationBuilder.RenameIndex(
                name: "IX_OfficerLocations_UserId",
                table: "OfficerLocations",
                newName: "IX_OfficerLocations_OfficerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Alerts",
                newName: "OfficerId");

            migrationBuilder.RenameIndex(
                name: "IX_Alerts_UserId",
                table: "Alerts",
                newName: "IX_Alerts_OfficerId");

            migrationBuilder.CreateTable(
                name: "Officers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Officers", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Officers_OfficerId",
                table: "Alerts",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfficerLocations_Officers_OfficerId",
                table: "OfficerLocations",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
