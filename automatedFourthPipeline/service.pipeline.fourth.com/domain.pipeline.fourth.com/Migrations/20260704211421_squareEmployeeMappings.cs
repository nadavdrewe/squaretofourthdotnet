using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace domain.pipeline.fourth.com.Migrations
{
    /// <inheritdoc />
    public partial class squareEmployeeMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SquareEmployeeMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreIntegrationId = table.Column<int>(type: "int", nullable: false),
                    SquareTeamMemberId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SquareDisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FourthEmployeeNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    WhenCreatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquareEmployeeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SquareEmployeeMappings_StoreIntegrations_StoreIntegrationId",
                        column: x => x.StoreIntegrationId,
                        principalTable: "StoreIntegrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SquareEmployeeMappings_StoreIntegrationId_SquareTeamMemberId_Active",
                table: "SquareEmployeeMappings",
                columns: new[] { "StoreIntegrationId", "SquareTeamMemberId" },
                unique: true,
                filter: "[Active] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SquareEmployeeMappings");
        }
    }
}
