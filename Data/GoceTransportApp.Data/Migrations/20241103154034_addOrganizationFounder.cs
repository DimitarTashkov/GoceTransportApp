using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addOrganizationFounder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Founder",
                table: "Organizations");

            migrationBuilder.AddColumn<string>(
                name: "FounderId",
                table: "Organizations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_FounderId",
                table: "Organizations",
                column: "FounderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AspNetUsers_FounderId",
                table: "Organizations",
                column: "FounderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AspNetUsers_FounderId",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_FounderId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "FounderId",
                table: "Organizations");

            migrationBuilder.AddColumn<string>(
                name: "Founder",
                table: "Organizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
