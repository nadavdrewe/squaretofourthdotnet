using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace domain.pipeline.fourth.com.Migrations
{
    /// <inheritdoc />
    public partial class pipelineRunRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PipelineRunRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: true),
                    BrandName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    StoreName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    StoreIntegrationId = table.Column<int>(type: "int", nullable: true),
                    SquareLocationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FourthUnitId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FourthLocationCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SourceSystem = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    TargetSystem = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PeriodStartUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEndUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OutputFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OutputFullPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    PayloadFormat = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FourthStatusCode = table.Column<int>(type: "int", nullable: true),
                    FourthResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhenCreatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineRunRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRunRecords_StoreIntegrationId_DataType_TransactionDate",
                table: "PipelineRunRecords",
                columns: new[] { "StoreIntegrationId", "DataType", "TransactionDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PipelineRunRecords");
        }
    }
}
