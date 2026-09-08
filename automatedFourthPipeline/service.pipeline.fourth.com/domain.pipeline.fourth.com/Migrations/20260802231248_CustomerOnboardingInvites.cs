using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace domain.pipeline.fourth.com.Migrations
{
    /// <inheritdoc />
    public partial class CustomerOnboardingInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerOnboardingInvites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    KeyHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    MaxUses = table.Column<int>(type: "int", nullable: false),
                    UseCount = table.Column<int>(type: "int", nullable: false),
                    ExpiresAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUsedUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RedeemedBrandId = table.Column<int>(type: "int", nullable: true),
                    RedeemedUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    WhenCreatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOnboardingInvites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOnboardingInvites_KeyHash",
                table: "CustomerOnboardingInvites",
                column: "KeyHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOnboardingInvites");
        }
    }
}
