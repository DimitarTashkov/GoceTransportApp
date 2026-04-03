using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FromLatitude",
                table: "Routes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FromLongitude",
                table: "Routes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromStopName",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ToLatitude",
                table: "Routes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ToLongitude",
                table: "Routes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToStopName",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromLatitude",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "FromLongitude",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "FromStopName",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ToLatitude",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ToLongitude",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ToStopName",
                table: "Routes");
        }
    }
}
