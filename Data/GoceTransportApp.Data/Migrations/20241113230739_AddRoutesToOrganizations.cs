using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoutesToOrganizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Routes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OrganizationId",
                table: "Routes",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Organizations_OrganizationId",
                table: "Routes",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Organizations_OrganizationId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_OrganizationId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Routes");
        }
    }
}
