//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using data.pipeline.fourth.com.Models.CredentialTypes;
//using data.pipeline.fourth.com.Models.Credentials;

//namespace com.fourth.pipeline.credentials.DataContext
//{
//    public class CredentialsContext : DbContext
//    {
//        const string connectionString = "Set ConnectionStrings__FourthSalesPipelineContextCreds in local configuration.";

//        /// <summary>
//        /// All creds are store here unless they need customer type. Query by store / brand and IntegrationCredentialTypes
//        /// </summary>
//        public DbSet<BaseCredential> CredentialsPool { get; set; }

//        public CredentialsContext()
//        {

//        }

//        public CredentialsContext(DbContextOptions<CredentialsContext> options) : base(options) { }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseSqlServer(connectionString);
//        }
//    }
//}
