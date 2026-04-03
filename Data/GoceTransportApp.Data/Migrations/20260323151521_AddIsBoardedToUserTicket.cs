using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBoardedToUserTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("0910a0b8-da04-4c92-bc10-5b72701cc10c"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("2cdc0d79-22db-46a9-81a6-9548d78b4869"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("5502e752-4d16-40cb-947e-584b1b64fd8b"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("b1a64ac9-84ca-4200-ad23-cc8997e13f34"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("e8d9d367-8847-426f-a92c-a2518666614a"));

            migrationBuilder.AddColumn<bool>(
                name: "IsBoarded",
                table: "UsersTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBoarded",
                table: "UsersTickets");

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "CreatedOn", "DeletedOn", "FounderId", "IsDeleted", "ModifiedOn", "Name", "Phone" },
                values: new object[,]
                {
                    { new Guid("0910a0b8-da04-4c92-bc10-5b72701cc10c"), "789 Oak St, Ogdenville", new DateTime(2025, 1, 18, 18, 40, 59, 487, DateTimeKind.Local).AddTicks(1331), null, "c40f52d6-d2cc-4379-8acf-782558dba84c", false, null, "Safe Travels", "+1 555-9012" },
                    { new Guid("2cdc0d79-22db-46a9-81a6-9548d78b4869"), "123 Main St, Springfield", new DateTime(2025, 1, 18, 18, 40, 59, 487, DateTimeKind.Local).AddTicks(1237), null, "4b5ab8a3-7604-447a-a603-f8e97e45fb61", false, null, "City Transport Co.", "+1 555-1234" },
                    { new Guid("5502e752-4d16-40cb-947e-584b1b64fd8b"), "456 Elm St, Shelbyville", new DateTime(2025, 1, 18, 18, 40, 59, 487, DateTimeKind.Local).AddTicks(1323), null, "a982c9b9-c1bc-4a5b-a89a-4c53a701c526", false, null, "Express Logistics", "+1 555-5678" },
                    { new Guid("b1a64ac9-84ca-4200-ad23-cc8997e13f34"), "321 Pine St, North Haverbrook", new DateTime(2025, 1, 18, 18, 40, 59, 487, DateTimeKind.Local).AddTicks(1338), null, "00591d7b-5bf5-475b-ac85-4a2467481881", false, null, "Urban Express", "+1 555-3456" },
                    { new Guid("e8d9d367-8847-426f-a92c-a2518666614a"), "654 Maple St, Capitol City", new DateTime(2025, 1, 18, 18, 40, 59, 487, DateTimeKind.Local).AddTicks(1345), null, "6b040313-77f4-49b0-b3d3-b25de95eb408", false, null, "Comfy Rides", "+1 555-7890" }
                });
        }
    }
}
