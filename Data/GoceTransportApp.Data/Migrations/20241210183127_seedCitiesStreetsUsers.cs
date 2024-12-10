using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class seedCitiesStreetsUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "City", "ConcurrencyStamp", "CreatedOn", "DeletedOn", "Email", "EmailConfirmed", "FirstName", "IsDeleted", "LastName", "LockoutEnabled", "LockoutEnd", "ModifiedOn", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "00591d7b-5bf5-475b-ac85-4a2467481881", 0, "Lqski", "d80d2cce-3bfd-412b-bb92-80bb374de37b", new DateTime(2024, 12, 10, 20, 31, 26, 963, DateTimeKind.Local).AddTicks(669), null, "user4@example.com", false, "David", false, "Brown", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user4" },
                    { "4b5ab8a3-7604-447a-a603-f8e97e45fb61", 0, "Gotse Delchev", "5d10b091-c232-450f-a0bc-c55af50485dd", new DateTime(2024, 12, 10, 20, 31, 26, 963, DateTimeKind.Local).AddTicks(510), null, "user1@example.com", false, "Alice", false, "Smith", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user1" },
                    { "6b040313-77f4-49b0-b3d3-b25de95eb408", 0, "Mosomishte", "9ba962c9-ae95-4f3b-8a01-fe1338f5b730", new DateTime(2024, 12, 10, 20, 31, 26, 963, DateTimeKind.Local).AddTicks(689), null, "user5@example.com", false, "Eve", false, "Davis", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user5" },
                    { "a982c9b9-c1bc-4a5b-a89a-4c53a701c526", 0, "Mosomishte", "3cf866a1-e9cd-499f-8220-36e3fecfe7a3", new DateTime(2024, 12, 10, 20, 31, 26, 963, DateTimeKind.Local).AddTicks(610), null, "user2@example.com", false, "Bob", false, "Johnson", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user2" },
                    { "c40f52d6-d2cc-4379-8acf-782558dba84c", 0, "Borovo", "85575b6a-1ad3-46cf-9f45-c6711d01d4f1", new DateTime(2024, 12, 10, 20, 31, 26, 963, DateTimeKind.Local).AddTicks(653), null, "user3@example.com", false, "Carol", false, "Taylor", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user3" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "ModifiedOn", "Name", "State", "ZipCode" },
                values: new object[,]
                {
                    { new Guid("3796f88d-d00b-4d50-9b53-7e1a9bcfee43"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Springfield", "Illinois", "62704" },
                    { new Guid("463fa98d-d831-4b20-a0d5-9e2c97e639b1"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Shelbyville", "Illinois", "62705" },
                    { new Guid("a8789eb6-4fea-4f81-a4cb-d15771913a83"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Capitol City", "Illinois", "62706" },
                    { new Guid("e471b8cb-70d6-4be3-9d01-c148188ba7c5"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Ogdenville", "Indiana", "46123" },
                    { new Guid("f70ac745-cd51-4813-92a2-8d066d5147d7"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "North Haverbrook", "Indiana", "46124" }
                });

            migrationBuilder.InsertData(
                table: "Streets",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("13bee015-1bab-434f-a1d2-be551805a887"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Polkovnik Drangov" },
                    { new Guid("1e2f6188-ea24-485a-8ae9-0ef2d4a76184"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Tsaritsa Joanna" },
                    { new Guid("466559a3-6508-4dc8-a9cb-22783cbe74a0"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Dimitar Talev" },
                    { new Guid("48e9172c-fc16-4458-a9e7-e336e7b4398c"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Qntra" },
                    { new Guid("e6c7ece3-710d-41f9-bbce-93e3d6a6aa0e"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Marica" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00591d7b-5bf5-475b-ac85-4a2467481881");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4b5ab8a3-7604-447a-a603-f8e97e45fb61");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6b040313-77f4-49b0-b3d3-b25de95eb408");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a982c9b9-c1bc-4a5b-a89a-4c53a701c526");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c40f52d6-d2cc-4379-8acf-782558dba84c");

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("3796f88d-d00b-4d50-9b53-7e1a9bcfee43"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("463fa98d-d831-4b20-a0d5-9e2c97e639b1"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("a8789eb6-4fea-4f81-a4cb-d15771913a83"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("e471b8cb-70d6-4be3-9d01-c148188ba7c5"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("f70ac745-cd51-4813-92a2-8d066d5147d7"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("13bee015-1bab-434f-a1d2-be551805a887"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("1e2f6188-ea24-485a-8ae9-0ef2d4a76184"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("466559a3-6508-4dc8-a9cb-22783cbe74a0"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("48e9172c-fc16-4458-a9e7-e336e7b4398c"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("e6c7ece3-710d-41f9-bbce-93e3d6a6aa0e"));
        }
    }
}
