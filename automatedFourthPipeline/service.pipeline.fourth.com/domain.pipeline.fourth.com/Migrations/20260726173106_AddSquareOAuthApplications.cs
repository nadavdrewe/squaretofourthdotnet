using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace domain.pipeline.fourth.com.Migrations
{
    /// <inheritdoc />
    public partial class AddSquareOAuthApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SquareOAuthApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(191)", maxLength: 191, nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    RedirectUri = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    WhenCreatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquareOAuthApplications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SquareOAuthApplications_Environment_ApplicationId",
                table: "SquareOAuthApplications",
                columns: new[] { "Environment", "ApplicationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SquareOAuthApplications");
        }
    }
}
