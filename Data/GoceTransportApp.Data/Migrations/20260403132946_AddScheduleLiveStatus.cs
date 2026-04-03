using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleLiveStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LiveStatus",
                table: "Schedules",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiveStatus",
                table: "Schedules");
        }
    }
}
