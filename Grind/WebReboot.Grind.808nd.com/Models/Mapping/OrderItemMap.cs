using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class OrderItemMap : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_orderitem_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("OrderItems");
            this.Property(t => t.DBKEY_orderitem_id).HasColumnName("DBKEY_orderitem_id");
            this.Property(t => t.catering_complete).HasColumnName("catering_complete");
            this.Property(t => t.commission).HasColumnName("commission");
            this.Property(t => t.cost).HasColumnName("cost");
            this.Property(t => t.course_number).HasColumnName("course_number");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.crv_value).HasColumnName("crv_value");
            this.Property(t => t.cup_qty).HasColumnName("cup_qty");
            this.Property(t => t.cup_weight).HasColumnName("cup_weight");
            this.Property(t => t.deleted).HasColumnName("deleted");
            this.Property(t => t.dining_option).HasColumnName("dining_option");
            this.Property(t => t.discount).HasColumnName("discount");
            this.Property(t => t.discount_amount).HasColumnName("discount_amount");
            this.Property(t => t.discount_reason).HasColumnName("discount_reason");
            this.Property(t => t.discount_rule_amount).HasColumnName("discount_rule_amount");
            this.Property(t => t.discount_taxed).HasColumnName("discount_taxed");
            this.Property(t => t.exchange_discount).HasColumnName("exchange_discount");
            this.Property(t => t.exchanged).HasColumnName("exchanged");
            this.Property(t => t.orderitem_id).HasColumnName("orderitem_id");
            this.Property(t => t.initial_price).HasColumnName("initial_price");
            this.Property(t => t.is_cold).HasColumnName("is_cold");
            this.Property(t => t.is_coupon).HasColumnName("is_coupon");
            this.Property(t => t.is_gift).HasColumnName("is_gift");
            this.Property(t => t.modifier_amount).HasColumnName("modifier_amount");
            this.Property(t => t.modifier_cost).HasColumnName("modifier_cost");
            this.Property(t => t.modifieritems).HasColumnName("modifieritems");
            this.Property(t => t.on_hold).HasColumnName("on_hold");
            this.Property(t => t.order).HasColumnName("order");
            this.Property(t => t.order_local_id).HasColumnName("order_local_id");
            this.Property(t => t.price).HasColumnName("price");
            this.Property(t => t.printed).HasColumnName("printed");
            this.Property(t => t.product).HasColumnName("product");
            this.Property(t => t.product_name_override).HasColumnName("product_name_override");
            this.Property(t => t.quantity).HasColumnName("quantity");
            this.Property(t => t.resource_uri).HasColumnName("resource_uri");
            this.Property(t => t.shared).HasColumnName("shared");
            this.Property(t => t.special_request).HasColumnName("special_request");
            this.Property(t => t.split_parts).HasColumnName("split_parts");
            this.Property(t => t.split_type).HasColumnName("split_type");
            this.Property(t => t.split_with_seat).HasColumnName("split_with_seat");
            this.Property(t => t.station).HasColumnName("station");
            this.Property(t => t.tax_amount).HasColumnName("tax_amount");
            this.Property(t => t.tax_rate).HasColumnName("tax_rate");
            this.Property(t => t.tax_rebate).HasColumnName("tax_rebate");
            this.Property(t => t.taxed_flag).HasColumnName("taxed_flag");
            this.Property(t => t.temp_sort).HasColumnName("temp_sort");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
            this.Property(t => t.uuid).HasColumnName("uuid");
            this.Property(t => t.voided_by).HasColumnName("voided_by");
            this.Property(t => t.voided_date).HasColumnName("voided_date");
            this.Property(t => t.voided_reason).HasColumnName("voided_reason");
            this.Property(t => t.weight).HasColumnName("weight");
            this.Property(t => t.total_price_after_tax).HasColumnName("total_price_after_tax");
            this.Property(t => t.total_price_after_discount).HasColumnName("total_price_after_discount");
            this.Property(t => t.parent_order_id).HasColumnName("parent_order_id");
            this.Property(t => t.product_id).HasColumnName("product_id");
            this.Property(t => t.pure_sales).HasColumnName("pure_sales");
            this.Property(t => t.Order_DBKEY_order_id).HasColumnName("Order_DBKEY_order_id");
            this.Property(t => t.discount_id).HasColumnName("discount_id");
            this.Property(t => t.expedited).HasColumnName("expedited");
            this.Property(t => t.kitchen_completed).HasColumnName("kitchen_completed");
            this.Property(t => t.start_time).HasColumnName("start_time");
        }
    }
}
