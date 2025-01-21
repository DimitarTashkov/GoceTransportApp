using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedIdForUserToContactForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ContactForms");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ContactForms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");


            migrationBuilder.CreateIndex(
                name: "IX_ContactForms_UserId",
                table: "ContactForms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactForms_AspNetUsers_UserId",
                table: "ContactForms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactForms_AspNetUsers_UserId",
                table: "ContactForms");

            migrationBuilder.DropIndex(
                name: "IX_ContactForms_UserId",
                table: "ContactForms");

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

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ContactForms");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ContactForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

        }
    }
}
