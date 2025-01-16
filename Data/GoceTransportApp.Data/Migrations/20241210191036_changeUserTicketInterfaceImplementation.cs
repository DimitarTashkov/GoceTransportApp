using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeUserTicketInterfaceImplementation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("032f645f-dfc1-40a0-b7a3-4069718a7f9a"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("8a2a2be4-3a63-467e-ad81-3378d221c4b1"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("9cd114f9-aa6f-4475-8a47-ae96ee8757fe"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("9ef2c95b-b71a-4c20-9ef8-2c118ba4d666"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("fe85e70b-8c1a-4bbb-aba6-85c6724786e2"));

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "UsersTickets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UsersTickets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "UsersTickets");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("14ead5ad-7377-491e-8c21-f64636666c7d"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("3cb689f3-e38d-4ae4-be46-4c8fbe3c1b33"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("6b4d82c9-e6d8-4d92-b172-c24c8548a74a"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("7fb75405-1620-48d0-9042-045aaa310d76"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("f598d81f-7a76-4929-ab19-47e7df16b186"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "UsersTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UsersTickets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "UsersTickets",
                type: "datetime2",
                nullable: true);


        }
    }
}
