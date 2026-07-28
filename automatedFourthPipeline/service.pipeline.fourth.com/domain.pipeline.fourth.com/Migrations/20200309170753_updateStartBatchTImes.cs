using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class updateStartBatchTImes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndBatchTimeUTC",
                table: "StoreIntegrations");

            migrationBuilder.DropColumn(
                name: "FireTimeUTC",
                table: "StoreIntegrations");

            migrationBuilder.DropColumn(
                name: "StartBatchTimeUTC",
                table: "StoreIntegrations");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "LightspeedRestoStoreConfig");

            migrationBuilder.AddColumn<int>(
                name: "StartBatchQueryTimeUTC",
                table: "BrandIntegrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartBatchQueryTimeUTC",
                table: "BrandIntegrations");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndBatchTimeUTC",
                table: "StoreIntegrations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FireTimeUTC",
                table: "StoreIntegrations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartBatchTimeUTC",
                table: "StoreIntegrations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "LightspeedRestoStoreConfig",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
