using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixTrafficSignalsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Signals",
                table: "Signals");

            migrationBuilder.RenameTable(
                name: "Signals",
                newName: "TrafficSignals");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TrafficSignals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrafficSignals",
                table: "TrafficSignals",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TrafficSignals",
                table: "TrafficSignals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TrafficSignals");

            migrationBuilder.RenameTable(
                name: "TrafficSignals",
                newName: "Signals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Signals",
                table: "Signals",
                column: "Id");
        }
    }
}
