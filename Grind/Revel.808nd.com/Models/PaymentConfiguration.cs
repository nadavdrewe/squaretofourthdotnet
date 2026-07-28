using System.Data.Entity.ModelConfiguration;

namespace Revel._808nd.com.Classes
{
    class PaymentConfiguration : EntityTypeConfiguration<Payment>
    {

        public PaymentConfiguration()
            : base()
        {
            HasKey(p => p.DBKEY_payment_id);
            ToTable("Payments");

        }
    }


    class DiscountConfig : EntityTypeConfiguration<Discount>
    {

        public DiscountConfig()
            : base()
        {
            HasKey(p => p.DBKEY_discount_id);
            ToTable("Discounts");

        }
    }

    class RewardsCardNewConfig : EntityTypeConfiguration<RewardsCardNew>
    {
        public RewardsCardNewConfig()
            : base()
        {
            HasKey(p => p.DBKEY_rewardscardnew_id);
            ToTable("RewardsCardNew");
        }


    }
    class CustomerConfig : EntityTypeConfiguration<Customer>
    {
        public CustomerConfig()
            : base()
        {
            HasKey(p => p.customer_id);
            ToTable("Customers");
        }


    }
}
