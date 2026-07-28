using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace domain.pipeline.fourth.com.Migrations
{
    /// <inheritdoc />
    public partial class pipelineEventLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PipelineEventLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PipelineRunRecordId = table.Column<int>(type: "int", nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
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
                    Stage = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PeriodStartUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PeriodEndUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ItemCount = table.Column<int>(type: "int", nullable: true),
                    RowCount = table.Column<int>(type: "int", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    OutputFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OutputFullPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhenCreatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineEventLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineEventLogs_CorrelationId_WhenCreatedUTC",
                table: "PipelineEventLogs",
                columns: new[] { "CorrelationId", "WhenCreatedUTC" });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineEventLogs_StoreIntegrationId_DataType_TransactionDate_WhenCreatedUTC",
                table: "PipelineEventLogs",
                columns: new[] { "StoreIntegrationId", "DataType", "TransactionDate", "WhenCreatedUTC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PipelineEventLogs");
        }
    }
}
