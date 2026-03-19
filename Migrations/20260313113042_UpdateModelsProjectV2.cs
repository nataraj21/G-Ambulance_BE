using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsProjectV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "AmbulanceLocation",
                newName: "UpdatedTime");

            migrationBuilder.AddColumn<double>(
                name: "Speed",
                table: "AmbulanceLocation",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "VehicleNumber",
                table: "AmbulanceLocation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TrafficSignals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SignalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficSignals", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrafficSignals");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "AmbulanceLocation");

            migrationBuilder.DropColumn(
                name: "VehicleNumber",
                table: "AmbulanceLocation");

            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "AmbulanceLocation",
                newName: "CreatedDate");
        }
    }
}
