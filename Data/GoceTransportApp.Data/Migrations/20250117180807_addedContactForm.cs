using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedContactForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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


            migrationBuilder.CreateTable(
                name: "ContactForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactForms", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "CreatedOn", "DeletedOn", "FounderId", "IsDeleted", "ModifiedOn", "Name", "Phone" },
                values: new object[,]
                {
                    { new Guid("1eec07d1-8672-4061-a00b-0aaa1ea814f8"), "456 Elm St, Shelbyville", new DateTime(2025, 1, 17, 20, 8, 6, 181, DateTimeKind.Local).AddTicks(2431), null, "a982c9b9-c1bc-4a5b-a89a-4c53a701c526", false, null, "Express Logistics", "+1 555-5678" },
                    { new Guid("67053609-0a06-4bd4-9c1e-ca310239cda8"), "789 Oak St, Ogdenville", new DateTime(2025, 1, 17, 20, 8, 6, 181, DateTimeKind.Local).AddTicks(2453), null, "c40f52d6-d2cc-4379-8acf-782558dba84c", false, null, "Safe Travels", "+1 555-9012" },
                    { new Guid("9ec1e90f-0448-4039-9fe6-5b0d1a099ef4"), "654 Maple St, Capitol City", new DateTime(2025, 1, 17, 20, 8, 6, 181, DateTimeKind.Local).AddTicks(2466), null, "6b040313-77f4-49b0-b3d3-b25de95eb408", false, null, "Comfy Rides", "+1 555-7890" },
                    { new Guid("a6a9f70c-ba13-473d-a92b-e04d89499c27"), "321 Pine St, North Haverbrook", new DateTime(2025, 1, 17, 20, 8, 6, 181, DateTimeKind.Local).AddTicks(2460), null, "00591d7b-5bf5-475b-ac85-4a2467481881", false, null, "Urban Express", "+1 555-3456" },
                    { new Guid("d391fb7f-e3b0-4490-a4bd-831e6624c740"), "123 Main St, Springfield", new DateTime(2025, 1, 17, 20, 8, 6, 181, DateTimeKind.Local).AddTicks(2342), null, "4b5ab8a3-7604-447a-a603-f8e97e45fb61", false, null, "City Transport Co.", "+1 555-1234" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactForms_IsDeleted",
                table: "ContactForms",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactForms");

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("1eec07d1-8672-4061-a00b-0aaa1ea814f8"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("67053609-0a06-4bd4-9c1e-ca310239cda8"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("9ec1e90f-0448-4039-9fe6-5b0d1a099ef4"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("a6a9f70c-ba13-473d-a92b-e04d89499c27"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("d391fb7f-e3b0-4490-a4bd-831e6624c740"));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "CreatedOn", "DeletedOn", "FounderId", "ImageUrl", "IsDeleted", "ModifiedOn", "Name", "Phone" },
                values: new object[,]
                {
                    { new Guid("14ead5ad-7377-491e-8c21-f64636666c7d"), "456 Elm St, Shelbyville", new DateTime(2024, 12, 10, 21, 10, 35, 180, DateTimeKind.Local).AddTicks(7675), null, "a982c9b9-c1bc-4a5b-a89a-4c53a701c526", "../../images/no-organization-image", false, null, "Express Logistics", "+1 555-5678" },
                    { new Guid("3cb689f3-e38d-4ae4-be46-4c8fbe3c1b33"), "654 Maple St, Capitol City", new DateTime(2024, 12, 10, 21, 10, 35, 180, DateTimeKind.Local).AddTicks(7702), null, "6b040313-77f4-49b0-b3d3-b25de95eb408", "../../images/no-organization-image", false, null, "Comfy Rides", "+1 555-7890" },
                    { new Guid("6b4d82c9-e6d8-4d92-b172-c24c8548a74a"), "321 Pine St, North Haverbrook", new DateTime(2024, 12, 10, 21, 10, 35, 180, DateTimeKind.Local).AddTicks(7691), null, "00591d7b-5bf5-475b-ac85-4a2467481881", "../../images/no-organization-image", false, null, "Urban Express", "+1 555-3456" },
                    { new Guid("7fb75405-1620-48d0-9042-045aaa310d76"), "123 Main St, Springfield", new DateTime(2024, 12, 10, 21, 10, 35, 180, DateTimeKind.Local).AddTicks(7612), null, "4b5ab8a3-7604-447a-a603-f8e97e45fb61", "../../images/no-organization-image", false, null, "City Transport Co.", "+1 555-1234" },
                    { new Guid("f598d81f-7a76-4929-ab19-47e7df16b186"), "789 Oak St, Ogdenville", new DateTime(2024, 12, 10, 21, 10, 35, 180, DateTimeKind.Local).AddTicks(7684), null, "c40f52d6-d2cc-4379-8acf-782558dba84c", "../../images/no-organization-image", false, null, "Safe Travels", "+1 555-9012" }
                });
        }
    }
}
