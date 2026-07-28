using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class lastestTUC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WhenCreatedUTC",
                table: "CredentialsPool",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenUpdatedUTC",
                table: "CredentialsPool",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenCreatedUTC",
                table: "Brands",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenUpdatedUTC",
                table: "Brands",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhenCreatedUTC",
                table: "CredentialsPool");

            migrationBuilder.DropColumn(
                name: "WhenUpdatedUTC",
                table: "CredentialsPool");

            migrationBuilder.DropColumn(
                name: "WhenCreatedUTC",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "WhenUpdatedUTC",
                table: "Brands");
        }
    }
}
