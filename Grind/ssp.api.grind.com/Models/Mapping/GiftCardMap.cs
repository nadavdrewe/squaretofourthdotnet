using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class GiftCardMap : EntityTypeConfiguration<GiftCard>
    {
        public GiftCardMap()
        {
            // Primary Key
            this.HasKey(t => t.giftcard_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("GiftCards");
            this.Property(t => t.giftcard_id).HasColumnName("giftcard_id");
            this.Property(t => t.address).HasColumnName("address");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.customer).HasColumnName("customer");
            this.Property(t => t.establishment).HasColumnName("establishment");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.initial_value).HasColumnName("initial_value");
            this.Property(t => t.number).HasColumnName("number");
            this.Property(t => t.payment_type).HasColumnName("payment_type");
            this.Property(t => t.remaining_balance).HasColumnName("remaining_balance");
            this.Property(t => t.resource_uri).HasColumnName("resource_uri");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
            this.Property(t => t.theAddress).HasColumnName("theAddress");
            this.Property(t => t.theCustomer_DBKEY_customer_id).HasColumnName("theCustomer_DBKEY_customer_id");
            this.Property(t => t.RewardsCardNew_DBKEY_rewardscardnew_id).HasColumnName("RewardsCardNew_DBKEY_rewardscardnew_id");
            this.Property(t => t.LinkingRevelCustomerID).HasColumnName("LinkingRevelCustomerID");
            this.Property(t => t.LinkingRevelRewardsCardNewID).HasColumnName("LinkingRevelRewardsCardNewID");

            // Relationships
            this.HasOptional(t => t.Customer1)
                .WithMany(t => t.GiftCards)
                .HasForeignKey(d => d.theCustomer_DBKEY_customer_id);
            this.HasOptional(t => t.RewardsCardNew)
                .WithMany(t => t.GiftCards)
                .HasForeignKey(d => d.RewardsCardNew_DBKEY_rewardscardnew_id);

        }
    }
}
