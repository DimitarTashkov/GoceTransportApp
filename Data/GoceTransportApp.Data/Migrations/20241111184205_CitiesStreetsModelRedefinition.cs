using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CitiesStreetsModelRedefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "CitiesStreets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "CitiesStreets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CitiesStreets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CitiesStreets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "CitiesStreets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CitiesStreets_IsDeleted",
                table: "CitiesStreets",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CitiesStreets_IsDeleted",
                table: "CitiesStreets");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "CitiesStreets");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "CitiesStreets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CitiesStreets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CitiesStreets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "CitiesStreets");
        }
    }
}
