using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class PaymentMap : EntityTypeConfiguration<Payment>
    {
        public PaymentMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_payment_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Payments");
            this.Property(t => t.DBKEY_payment_id).HasColumnName("DBKEY_payment_id");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.amount_authorized).HasColumnName("amount_authorized");
            this.Property(t => t.card_type).HasColumnName("card_type");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.deleted).HasColumnName("deleted");
            this.Property(t => t.establishment).HasColumnName("establishment");
            this.Property(t => t.executed).HasColumnName("executed");
            this.Property(t => t.order).HasColumnName("order");
            this.Property(t => t.payment_date).HasColumnName("payment_date");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
            this.Property(t => t.order_id).HasColumnName("order_id");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
            this.Property(t => t.cc_first_name).HasColumnName("cc_first_name");
            this.Property(t => t.cc_last_name).HasColumnName("cc_last_name");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.first_4_cc_digits).HasColumnName("first_4_cc_digits");
            this.Property(t => t.last_4_cc_digits).HasColumnName("last_4_cc_digits");
            this.Property(t => t.other_payment_type).HasColumnName("other_payment_type");
            this.Property(t => t.payment_type).HasColumnName("payment_type");
            this.Property(t => t.refund_transaction_id).HasColumnName("refund_transaction_id");
        }
    }
}
