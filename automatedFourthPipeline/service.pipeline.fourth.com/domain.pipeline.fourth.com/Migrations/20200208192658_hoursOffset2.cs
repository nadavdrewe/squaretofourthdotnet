using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class hoursOffset2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UTCOffset",
                table: "Stores");

            migrationBuilder.AddColumn<int>(
                name: "UTCOffsetInHours",
                table: "Stores",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UTCOffsetInHours",
                table: "Stores");

            migrationBuilder.AddColumn<int>(
                name: "UTCOffset",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
