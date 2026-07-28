using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class Addacceesstoken1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "CredentialsPool",
                newName: "ClientSecret");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientSecret",
                table: "CredentialsPool",
                newName: "ApplicationId");
        }
    }
}
