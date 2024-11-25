using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Drivers");
        }
    }
}
