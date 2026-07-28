using System.Data.Common;
using System.Data.Entity;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.ReportingModel;

namespace Revel._808nd.com.Models
{
    public partial class GrindContext : RevelContextBase, IRevelDBContextable
    {

        public IDbSet<OrderItemExport> OrderItemExports { get; set; }
        public IDbSet<OrderItemExportExcludeCompsAndVoids> OrderItemExportExcludeCompsAndVoids { get; set; }
        public IDbSet<BlackCardSignup> BlackCardSignups { get; set; }
        public IDbSet<ApiAuthentication> ApiAuthentications { get; set; }
        public IDbSet<WifiLogin> WifiLogins { get; set; }
        public IDbSet<MenuPermissions> MenuPermissions { get; set; }
        public IDbSet<InvestorCardHolder> InvestorCardHolders { get; set; }
        public IDbSet<Funding> Fundings { get; set; }
        public IDbSet<_445Calendar> _445Calendar { get; set; }
        public IDbSet<CashupNotifier> CashupNotifiers { get; set; }
        public IDbSet<OpeningHours> OpeningHours { get; set; }
        public IDbSet<ProjectionType> ProjectionTypes { get; set; }
        public IDbSet<Projection> Projections { get; set; }
        public IDbSet<SystemLog> SystemLogs { get; set; }   
        public IDbSet<User> Users { get; set; }
        public IDbSet<ProductWatch> ProductWatches { get; set; }

        public IDbSet<MiscSettings> MiscSettings { get; set; }

        static GrindContext()
        {
            Database.SetInitializer<GrindContext>(null);

        }

        public GrindContext()
            : base("Name=GrindContext")
        {
            Database.CommandTimeout = 0;
        }


        public GrindContext(string name) : base(name)
        {
            Database.CommandTimeout = 0;
        }



        public GrindContext(DbConnection dbConnection)
            : base(dbConnection)
        {

            Database.CommandTimeout = 0;
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //set some precisions for the decimals

            modelBuilder.Entity<Customer>().HasKey(t => t.DBKEY_customer_id);
            modelBuilder.Entity<RewardsCardNew>().HasKey(t => t.DBKEY_rewardscardnew_id);
            modelBuilder.Entity<RewardsCardNew>().HasOptional(t => t.LoyaltyCardType);

            
            modelBuilder.Entity<OrderItem>().Property(product => product.price).HasPrecision(18, 10);
            modelBuilder.Entity<OrderItem>().Property(product => product.pure_sales).HasPrecision(18, 10);
            modelBuilder.Entity<OrderItem>().Property(product => product.tax_amount).HasPrecision(18, 10);
            modelBuilder.Configurations.Add(new PaymentConfiguration());
            modelBuilder.Entity<Payment>().Property(product => product.amount).HasPrecision(18, 10);
            modelBuilder.Entity<Payment>().Property(product => product.amount_authorized).HasPrecision(18, 10);

            modelBuilder.Configurations.Add(new DiscountConfig());


            modelBuilder.Entity<OrderItemExport>().Property(product => product.pure_sales).HasPrecision(18, 10);
            modelBuilder.Entity<OrderItemExport>().Property(product => product.price).HasPrecision(18, 10);
            modelBuilder.Entity<OrderItemExport>().Property(product => product.tax_amount).HasPrecision(18, 10);

            modelBuilder.Entity<OrderItemExportExcludeCompsAndVoids>().Property(product => product.pure_sales).HasPrecision(18, 10);
            modelBuilder.Entity<OrderItemExportExcludeCompsAndVoids>().Property(product => product.price).HasPrecision(18, 10);
            modelBuilder.Entity<OrderItemExportExcludeCompsAndVoids>().Property(product => product.tax_amount).HasPrecision(18, 10);


        }

    }
}
