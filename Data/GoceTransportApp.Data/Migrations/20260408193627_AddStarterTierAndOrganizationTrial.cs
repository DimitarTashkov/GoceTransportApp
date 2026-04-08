using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStarterTierAndOrganizationTrial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnTrial",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialExpiresOn",
                table: "Organizations",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnTrial",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "TrialExpiresOn",
                table: "Organizations");
        }
    }
}
