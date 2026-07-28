using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class currencyLSConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "LightspeedRestoStoreConfig",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "LightspeedRestoStoreConfig");
        }
    }
}
