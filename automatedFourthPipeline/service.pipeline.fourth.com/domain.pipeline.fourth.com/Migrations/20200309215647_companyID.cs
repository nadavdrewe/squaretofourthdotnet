using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class companyID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LsRestoCompanyId",
                table: "Stores");

            migrationBuilder.AddColumn<int>(
                name: "LsRestoCompanyId",
                table: "LightspeedRestoStoreConfig",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LsRestoCompanyId",
                table: "LightspeedRestoStoreConfig");

            migrationBuilder.AddColumn<int>(
                name: "LsRestoCompanyId",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
