using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAmbulanceNameFromLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmbulanceName",
                table: "Ambulances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AmbulanceName",
                table: "Ambulances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
