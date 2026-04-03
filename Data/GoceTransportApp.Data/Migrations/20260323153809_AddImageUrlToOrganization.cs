using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ImageUrl' AND Object_ID = Object_ID(N'Organizations'))
                  BEGIN
                      ALTER TABLE [Organizations] ADD [ImageUrl] nvarchar(max) NULL;
                  END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ImageUrl' AND Object_ID = Object_ID(N'Organizations'))
                  BEGIN
                      ALTER TABLE [Organizations] DROP COLUMN [ImageUrl];
                  END");
        }
    }
}
