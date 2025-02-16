using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class seedOrganizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "City", "ConcurrencyStamp", "CreatedOn", "DeletedOn", "Email", "EmailConfirmed", "FirstName", "IsDeleted", "LastName", "LockoutEnabled", "LockoutEnd", "ModifiedOn", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "3099c591-09ea-4c4f-ad94-8e4c557c1755", 0, "Lqski", "548ae479-2ccb-4ee6-b226-23ab7698626a", new DateTime(2024, 12, 10, 20, 39, 24, 270, DateTimeKind.Local).AddTicks(1088), null, "user4@example.com", false, "David", false, "Brown", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user4" },
                    { "42aa7029-877a-4ebf-9270-5109ac52405a", 0, "Mosomishte", "9ed57408-ea48-4d6a-a4d5-1049a118911c", new DateTime(2024, 12, 10, 20, 39, 24, 270, DateTimeKind.Local).AddTicks(1052), null, "user2@example.com", false, "Bob", false, "Johnson", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user2" },
                    { "88094558-c0f4-47a2-9f53-6797425d706d", 0, "Borovo", "ab3e1e6f-b27e-4b6f-87da-0be58c9e2931", new DateTime(2024, 12, 10, 20, 39, 24, 270, DateTimeKind.Local).AddTicks(1068), null, "user3@example.com", false, "Carol", false, "Taylor", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user3" },
                    { "d1e4340d-0a6c-497f-ae2c-574f03f7351a", 0, "Mosomishte", "3283f2a4-a79d-42ed-84d5-e32598fecaab", new DateTime(2024, 12, 10, 20, 39, 24, 270, DateTimeKind.Local).AddTicks(1104), null, "user5@example.com", false, "Eve", false, "Davis", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user5" },
                    { "fddd8eae-3ce9-450c-ba91-c34aaac73c82", 0, "Gotse Delchev", "44485c71-de49-4112-ab37-d3b56f9d93df", new DateTime(2024, 12, 10, 20, 39, 24, 270, DateTimeKind.Local).AddTicks(966), null, "user1@example.com", false, "Alice", false, "Smith", false, null, null, null, null, null, null, false, "../../images/no-profile-image", null, false, "user1" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "ModifiedOn", "Name", "State", "ZipCode" },
                values: new object[,]
                {
                    { new Guid("0bb97157-9f48-4019-9d57-4f8f08f56ea4"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Ogdenville", "Indiana", "46123" },
                    { new Guid("6ee1013b-9ef3-4abd-9f91-ee39a1bdc3bf"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Capitol City", "Illinois", "62706" },
                    { new Guid("a2326550-d8b7-48cd-afab-370f702febb5"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Springfield", "Illinois", "62704" },
                    { new Guid("cd658fe2-309d-4de5-8eae-5af9bd2dd637"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "North Haverbrook", "Indiana", "46124" },
                    { new Guid("ebf4be4c-b944-4a60-8820-ce9add91c913"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Shelbyville", "Illinois", "62705" }
                });


            migrationBuilder.InsertData(
                table: "Streets",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "IsDeleted", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("3b46a044-4bb2-4ad1-86f0-b198719e9137"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Polkovnik Drangov" },
                    { new Guid("48546194-3244-4c88-b55b-b3f1a10cfae4"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Tsaritsa Joanna" },
                    { new Guid("5ba431ef-d79a-4ad6-bcc6-9c9f382faec7"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Qntra" },
                    { new Guid("a64d591d-d8cf-488d-ad9a-dfb3a6962450"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Marica" },
                    { new Guid("c42a1b2e-afbe-4f81-8d68-aaac514f87af"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, "Dimitar Talev" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3099c591-09ea-4c4f-ad94-8e4c557c1755");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "42aa7029-877a-4ebf-9270-5109ac52405a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "88094558-c0f4-47a2-9f53-6797425d706d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1e4340d-0a6c-497f-ae2c-574f03f7351a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fddd8eae-3ce9-450c-ba91-c34aaac73c82");

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("0bb97157-9f48-4019-9d57-4f8f08f56ea4"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("6ee1013b-9ef3-4abd-9f91-ee39a1bdc3bf"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("a2326550-d8b7-48cd-afab-370f702febb5"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("cd658fe2-309d-4de5-8eae-5af9bd2dd637"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("ebf4be4c-b944-4a60-8820-ce9add91c913"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("692b4dc6-9c52-4a86-8dd0-70c70d2618b7"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("c29a68cd-bf19-4f47-b0be-e4c19c7d548e"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("c4fbbac9-ca54-4d2c-8a23-a1a072a77e46"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("c67f1eb3-11fd-4e34-a543-3f0ead3713fa"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("f9eef093-95c1-40b1-80c8-60c73e8045e3"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("3b46a044-4bb2-4ad1-86f0-b198719e9137"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("48546194-3244-4c88-b55b-b3f1a10cfae4"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("5ba431ef-d79a-4ad6-bcc6-9c9f382faec7"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("a64d591d-d8cf-488d-ad9a-dfb3a6962450"));

            migrationBuilder.DeleteData(
                table: "Streets",
                keyColumn: "Id",
                keyValue: new Guid("c42a1b2e-afbe-4f81-8d68-aaac514f87af"));

           

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
    }
}
