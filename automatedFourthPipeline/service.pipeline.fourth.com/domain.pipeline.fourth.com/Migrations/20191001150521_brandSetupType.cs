using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class brandSetupType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InitalSetupSytemType",
                table: "Brands",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitalSetupSytemType",
                table: "Brands");
        }
    }
}
