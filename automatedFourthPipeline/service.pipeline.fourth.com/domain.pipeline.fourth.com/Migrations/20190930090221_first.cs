using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace domain.pipeline.fourth.com.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ContactEmail = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Region = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Globals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Globals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrandIntegrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrandId = table.Column<int>(nullable: false),
                    IntegrationType = table.Column<int>(nullable: false),
                    IntegrationSubType = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandIntegrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandIntegrations_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Timezone = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    BrandId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialsPool",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    BaseEndpoint = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    KeySecret = table.Column<string>(nullable: true),
                    SupplimentalData1 = table.Column<string>(nullable: true),
                    SupplimentalData2 = table.Column<string>(nullable: true),
                    CredentialType = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    BrandId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialsPool", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialsPool_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CredentialsPool_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",                        
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreIntegrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartBatchTimeUTC = table.Column<DateTime>(nullable: false),
                    EndBatchTimeUTC = table.Column<DateTime>(nullable: false),
                    FireTimeUTC = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    IntegrationType = table.Column<int>(nullable: false),
                    IntegrationSubType = table.Column<int>(nullable: false),
                    WhenCreatedUTC = table.Column<DateTime>(nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(nullable: false),
                    StoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreIntegrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreIntegrations_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FourthSalesApiStoreConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UnitId = table.Column<string>(nullable: true),
                    SiteLocationCode = table.Column<string>(nullable: true),
                    RevenueCenter = table.Column<string>(nullable: true),
                    RevenueCenterMappingType = table.Column<int>(nullable: false),
                    StoreIntegrationId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    WhenCreatedUTC = table.Column<DateTime>(nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FourthSalesApiStoreConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FourthSalesApiStoreConfig_StoreIntegrations_StoreIntegrationId",
                        column: x => x.StoreIntegrationId,
                        principalTable: "StoreIntegrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LightspeedRestoStoreConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StoreIntegrationId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    WhenCreatedUTC = table.Column<DateTime>(nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LightspeedRestoStoreConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LightspeedRestoStoreConfig_StoreIntegrations_StoreIntegrationId",
                        column: x => x.StoreIntegrationId,
                        principalTable: "StoreIntegrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevelStoreConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EstablishmentID = table.Column<string>(nullable: true),
                    EstablishmentResourceUri = table.Column<string>(nullable: true),
                    StoreIntegrationId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    WhenCreatedUTC = table.Column<DateTime>(nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevelStoreConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevelStoreConfigs_StoreIntegrations_StoreIntegrationId",
                        column: x => x.StoreIntegrationId,
                        principalTable: "StoreIntegrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SquareStoreConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocationId = table.Column<string>(nullable: true),
                    StoreIntegrationId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    WhenCreatedUTC = table.Column<DateTime>(nullable: false),
                    WhenUpdatedUTC = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquareStoreConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SquareStoreConfigs_StoreIntegrations_StoreIntegrationId",
                        column: x => x.StoreIntegrationId,
                        principalTable: "StoreIntegrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevenueCenterCategoryMappings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryName = table.Column<string>(nullable: true),
                    CategoryId = table.Column<string>(nullable: true),
                    FourthSalesApiStoreConfigId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenueCenterCategoryMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevenueCenterCategoryMappings_FourthSalesApiStoreConfig_FourthSalesApiStoreConfigId",
                        column: x => x.FourthSalesApiStoreConfigId,
                        principalTable: "FourthSalesApiStoreConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandIntegrations_BrandId",
                table: "BrandIntegrations",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CredentialsPool_BrandId",
                table: "CredentialsPool",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CredentialsPool_StoreId",
                table: "CredentialsPool",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_FourthSalesApiStoreConfig_StoreIntegrationId",
                table: "FourthSalesApiStoreConfig",
                column: "StoreIntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_LightspeedRestoStoreConfig_StoreIntegrationId",
                table: "LightspeedRestoStoreConfig",
                column: "StoreIntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_RevelStoreConfigs_StoreIntegrationId",
                table: "RevelStoreConfigs",
                column: "StoreIntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueCenterCategoryMappings_FourthSalesApiStoreConfigId",
                table: "RevenueCenterCategoryMappings",
                column: "FourthSalesApiStoreConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SquareStoreConfigs_StoreIntegrationId",
                table: "SquareStoreConfigs",
                column: "StoreIntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreIntegrations_StoreId",
                table: "StoreIntegrations",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BrandId",
                table: "Stores",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandIntegrations");

            migrationBuilder.DropTable(
                name: "CredentialsPool");

            migrationBuilder.DropTable(
                name: "Globals");

            migrationBuilder.DropTable(
                name: "LightspeedRestoStoreConfig");

            migrationBuilder.DropTable(
                name: "RevelStoreConfigs");

            migrationBuilder.DropTable(
                name: "RevenueCenterCategoryMappings");

            migrationBuilder.DropTable(
                name: "SquareStoreConfigs");

            migrationBuilder.DropTable(
                name: "FourthSalesApiStoreConfig");

            migrationBuilder.DropTable(
                name: "StoreIntegrations");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
