using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class newAtts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationId",
                table: "CredentialsPool",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "CredentialsPool",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "CredentialsPool",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "CredentialsPool");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "CredentialsPool");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "CredentialsPool");
        }
    }
}
