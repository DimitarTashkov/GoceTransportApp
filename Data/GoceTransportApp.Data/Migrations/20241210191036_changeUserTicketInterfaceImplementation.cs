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

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "CreatedOn", "DeletedOn", "FounderId", "ImageUrl", "IsDeleted", "ModifiedOn", "Name", "Phone" },
                values: new object[,]
                {
                    { new Guid("032f645f-dfc1-40a0-b7a3-4069718a7f9a"), "654 Maple St, Capitol City", new DateTime(2024, 12, 10, 21, 8, 32, 223, DateTimeKind.Local).AddTicks(6039), null, "6b040313-77f4-49b0-b3d3-b25de95eb408", "../../images/no-organization-image", false, null, "Comfy Rides", "+1 555-7890" },
                    { new Guid("8a2a2be4-3a63-467e-ad81-3378d221c4b1"), "456 Elm St, Shelbyville", new DateTime(2024, 12, 10, 21, 8, 32, 223, DateTimeKind.Local).AddTicks(5980), null, "a982c9b9-c1bc-4a5b-a89a-4c53a701c526", "../../images/no-organization-image", false, null, "Express Logistics", "+1 555-5678" },
                    { new Guid("9cd114f9-aa6f-4475-8a47-ae96ee8757fe"), "789 Oak St, Ogdenville", new DateTime(2024, 12, 10, 21, 8, 32, 223, DateTimeKind.Local).AddTicks(6008), null, "c40f52d6-d2cc-4379-8acf-782558dba84c", "../../images/no-organization-image", false, null, "Safe Travels", "+1 555-9012" },
                    { new Guid("9ef2c95b-b71a-4c20-9ef8-2c118ba4d666"), "321 Pine St, North Haverbrook", new DateTime(2024, 12, 10, 21, 8, 32, 223, DateTimeKind.Local).AddTicks(6016), null, "00591d7b-5bf5-475b-ac85-4a2467481881", "../../images/no-organization-image", false, null, "Urban Express", "+1 555-3456" },
                    { new Guid("fe85e70b-8c1a-4bbb-aba6-85c6724786e2"), "123 Main St, Springfield", new DateTime(2024, 12, 10, 21, 8, 32, 223, DateTimeKind.Local).AddTicks(5912), null, "4b5ab8a3-7604-447a-a603-f8e97e45fb61", "../../images/no-organization-image", false, null, "City Transport Co.", "+1 555-1234" }
                });
        }
    }
}
