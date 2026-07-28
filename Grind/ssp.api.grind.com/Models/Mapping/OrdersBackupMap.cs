using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class OrdersBackupMap : EntityTypeConfiguration<OrdersBackup>
    {
        public OrdersBackupMap()
        {
            // Primary Key
            this.HasKey(t => new { t.order_id, t.bills_type, t.closed, t.created_date, t.crv_taxed, t.crv_value, t.dining_option, t.discount_amount, t.discount_tax_amount, t.exchange_discount, t.exchanged, t.final_total, t.gratuity_type, t.has_delivery_info, t.notification_email_sent, t.notification_text_sent, t.number_of_people, t.points_added, t.points_redeemed, t.prevailing_surcharge, t.prevailing_tax, t.printed, t.remaining_due, t.rounding_delta, t.service_charge, t.subtotal, t.surcharge, t.tax, t.tax_rebate, t.web_order, t.establishment_id });

            // Properties
            this.Property(t => t.order_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.bills_type)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.crv_value)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.dining_option)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.discount_amount)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.discount_tax_amount)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.final_total)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.gratuity_type)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.number_of_people)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.points_added)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.points_redeemed)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.prevailing_surcharge)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.prevailing_tax)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.remaining_due)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.rounding_delta)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.service_charge)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.subtotal)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.surcharge)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.tax)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.tax_rebate)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.establishment_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("OrdersBackup");
            this.Property(t => t.order_id).HasColumnName("order_id");
            this.Property(t => t.asap).HasColumnName("asap");
            this.Property(t => t.auto_grat_pct).HasColumnName("auto_grat_pct");
            this.Property(t => t.bill_number).HasColumnName("bill_number");
            this.Property(t => t.bills_info).HasColumnName("bills_info");
            this.Property(t => t.bills_type).HasColumnName("bills_type");
            this.Property(t => t.call_name).HasColumnName("call_name");
            this.Property(t => t.closed).HasColumnName("closed");
            this.Property(t => t.created_at).HasColumnName("created_at");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.crv_taxed).HasColumnName("crv_taxed");
            this.Property(t => t.crv_value).HasColumnName("crv_value");
            this.Property(t => t.dining_option).HasColumnName("dining_option");
            this.Property(t => t.discount).HasColumnName("discount");
            this.Property(t => t.discount_amount).HasColumnName("discount_amount");
            this.Property(t => t.discount_reason).HasColumnName("discount_reason");
            this.Property(t => t.discount_rule_amount).HasColumnName("discount_rule_amount");
            this.Property(t => t.discount_rule_type).HasColumnName("discount_rule_type");
            this.Property(t => t.discount_tax_amount).HasColumnName("discount_tax_amount");
            this.Property(t => t.discount_taxed).HasColumnName("discount_taxed");
            this.Property(t => t.establishment).HasColumnName("establishment");
            this.Property(t => t.exchange_discount).HasColumnName("exchange_discount");
            this.Property(t => t.exchanged).HasColumnName("exchanged");
            this.Property(t => t.final_total).HasColumnName("final_total");
            this.Property(t => t.gift_reward_data).HasColumnName("gift_reward_data");
            this.Property(t => t.gratuity).HasColumnName("gratuity");
            this.Property(t => t.gratuity_type).HasColumnName("gratuity_type");
            this.Property(t => t.has_delivery_info).HasColumnName("has_delivery_info");
            this.Property(t => t.is_discounted).HasColumnName("is_discounted");
            this.Property(t => t.is_unpaid).HasColumnName("is_unpaid");
            this.Property(t => t.local_id).HasColumnName("local_id");
            this.Property(t => t.notes).HasColumnName("notes");
            this.Property(t => t.notification_email_sent).HasColumnName("notification_email_sent");
            this.Property(t => t.notification_text_sent).HasColumnName("notification_text_sent");
            this.Property(t => t.number_of_people).HasColumnName("number_of_people");
            this.Property(t => t.points_added).HasColumnName("points_added");
            this.Property(t => t.points_redeemed).HasColumnName("points_redeemed");
            this.Property(t => t.pos_mode).HasColumnName("pos_mode");
            this.Property(t => t.prevailing_surcharge).HasColumnName("prevailing_surcharge");
            this.Property(t => t.prevailing_tax).HasColumnName("prevailing_tax");
            this.Property(t => t.printed).HasColumnName("printed");
            this.Property(t => t.remaining_due).HasColumnName("remaining_due");
            this.Property(t => t.resource_uri).HasColumnName("resource_uri");
            this.Property(t => t.rounding_delta).HasColumnName("rounding_delta");
            this.Property(t => t.service_charge).HasColumnName("service_charge");
            this.Property(t => t.subtotal).HasColumnName("subtotal");
            this.Property(t => t.surcharge).HasColumnName("surcharge");
            this.Property(t => t.tax).HasColumnName("tax");
            this.Property(t => t.tax_country).HasColumnName("tax_country");
            this.Property(t => t.tax_rebate).HasColumnName("tax_rebate");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
            this.Property(t => t.uuid).HasColumnName("uuid");
            this.Property(t => t.web_order).HasColumnName("web_order");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
        }
    }
}
