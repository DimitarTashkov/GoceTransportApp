using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_FromCityId",
                table: "Routes");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Departure",
                table: "Schedules",
                column: "Departure");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_FromCityId_ToCityId",
                table: "Routes",
                columns: new[] { "FromCityId", "ToCityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_Departure",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Routes_FromCityId_ToCityId",
                table: "Routes");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_FromCityId",
                table: "Routes",
                column: "FromCityId");
        }
    }
}
