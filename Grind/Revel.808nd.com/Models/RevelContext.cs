using System.Data.Entity;
using Revel._808nd.com.Classes;

namespace Revel._808nd.com.Models
{
public partial class RevelContext : RevelContextBase
    {

       /* public  virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<OpeningHours> OpeningHours { get; set; }
        public virtual DbSet<Establishment> Establishments { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<Discount> Discounts { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual  DbSet<RewardsCardNew> RewardsCardNew { get; set; }

        public virtual DbSet<GiftCard> GiftCards { get; set; }

        public virtual DbSet<RewardsCardDailyPoints> RewardsCardDailyPoints { get; set; }

        public virtual DbSet<RewardsPointsMultiplier> RewardsPointsMultiplier { get; set; }

        public virtual DbSet<RewardCardPointsTransactionLog> RewardCardPointsTransactionLogs { get; set; }

        public virtual DbSet<ScheduledTaskLog> ScheduledTaskLogs { get; set; }

        public virtual DbSet<Brand> Brands { get; set; }
*/

        static RevelContext()
        {
            Database.SetInitializer<RevelContext>(null);
        }

        public RevelContext()
            : base("Name=RevelContext")            
        {
            this.Database.CommandTimeout = 0;
        }

        public RevelContext(string connectionName)
          : base(connectionName)
        {
            this.Database.CommandTimeout = 0;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //set some precisions for the decimals

            modelBuilder.Entity<Customer>().HasKey(t => t.DBKEY_customer_id);
            modelBuilder.Entity<RewardsCardNew>().HasKey(t => t.DBKEY_rewardscardnew_id);

            modelBuilder.Entity<OrderItem>().Property(product => product.price).HasPrecision(18, 10);
            modelBuilder.Entity<OrderItem>().Property(product => product.pure_sales).HasPrecision(18,10);
            modelBuilder.Entity<OrderItem>().Property(product => product.tax_amount).HasPrecision(18, 10);
            modelBuilder.Configurations.Add(new PaymentConfiguration());
            modelBuilder.Entity<Payment>().Property(product => product.amount).HasPrecision(18, 10);
            modelBuilder.Entity<Payment>().Property(product => product.amount_authorized).HasPrecision(18, 10);

            modelBuilder.Configurations.Add(new DiscountConfig());





        }
    }
}
