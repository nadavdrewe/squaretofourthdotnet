using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class refreshOTken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "CredentialsPool",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "CredentialsPool",
                newName: "LatestAccessToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "CredentialsPool",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "LatestAccessToken",
                table: "CredentialsPool",
                newName: "AccessToken");
        }
    }
}
