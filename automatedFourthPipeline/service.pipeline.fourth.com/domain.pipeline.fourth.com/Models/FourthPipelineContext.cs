using System;
using com.fourth.pipeline.pos.Model;
using data.pipeline.fourth.com.Models;
using data.pipeline.fourth.com.Models.Configs;
using data.pipeline.fourth.com.Models.Configs.Store;
using data.pipeline.fourth.com.Models.Credentials;
using data.pipeline.fourth.com.Models.CredentialTypes;
using data.pipeline.fourth.com.Models.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace domain.pipeline.fourth.com.Models
{
    public class FourthPipelineContext : DbContext
    {
        public const string connectionString = "";

        public DbSet<Global> Globals { get; set; }
        public DbSet<Brand> Brands { get; set; }
        //Integrations
        public DbSet<BrandIntegration> BrandIntegrations { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreIntegration> StoreIntegrations { get; set; }
        public DbSet<PipelineRunRecord> PipelineRunRecords { get; set; }
        public DbSet<PipelineEventLog> PipelineEventLogs { get; set; }

        /// <summary>
        /// Revel Store Config for Revel integrations
        /// </summary>
        public DbSet<RevelStoreConfig> RevelStoreConfigs { get; set; }
        /// <summary>
        /// LS Store config for LS Resto
        /// </summary>
        public DbSet<LightspeedRestoStoreConfig> LightspeedRestoStoreConfig { get; set; }
        /// <summary>
        /// Fourth Sales API configs
        /// </summary>
        public DbSet<FourthSalesApiStoreConfig> FourthSalesApiStoreConfig { get; set; }

        public DbSet<SquareStoreConfig> SquareStoreConfigs { get; set; }
        public DbSet<SquareEmployeeMapping> SquareEmployeeMappings { get; set; }

        public DbSet<RevenueCenterCategoryMapping> RevenueCenterCategoryMappings { get; set; }


        /// <summary>
        /// All creds are store here unless they need customer type. Query by store / brand and IntegrationCredentialTypes
        /// </summary>
        public DbSet<BaseCredential> CredentialsPool { get; set; }
        public DbSet<SquareOAuthApplication> SquareOAuthApplications { get; set; }

        public FourthPipelineContext()
        {

        }

        public FourthPipelineContext(DbContextOptions<FourthPipelineContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuredConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__FourthSalesPipelineContext");
                if (string.IsNullOrWhiteSpace(configuredConnectionString))
                {
                    throw new InvalidOperationException("Set ConnectionStrings__FourthSalesPipelineContext when creating FourthPipelineContext without configured options.");
                }

                optionsBuilder.UseSqlServer(configuredConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SquareEmployeeMapping>()
                .HasIndex(x => new { x.StoreIntegrationId, x.SquareTeamMemberId })
                .HasDatabaseName("IX_SquareEmployeeMappings_StoreIntegrationId_SquareTeamMemberId_Active")
                .IsUnique()
                .HasFilter("[Active] = 1");

            modelBuilder.Entity<SquareOAuthApplication>()
                .HasIndex(x => new { x.Environment, x.ApplicationId })
                .IsUnique();

            modelBuilder.Entity<PipelineRunRecord>()
                .HasIndex(x => new { x.StoreIntegrationId, x.DataType, x.TransactionDate });

            modelBuilder.Entity<PipelineEventLog>()
                .HasIndex(x => new { x.StoreIntegrationId, x.DataType, x.TransactionDate, x.WhenCreatedUTC });

            modelBuilder.Entity<PipelineEventLog>()
                .HasIndex(x => new { x.CorrelationId, x.WhenCreatedUTC });
        }

    }
}
