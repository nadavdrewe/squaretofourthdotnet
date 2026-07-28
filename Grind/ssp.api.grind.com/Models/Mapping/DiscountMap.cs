using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class DiscountMap : EntityTypeConfiguration<Discount>
    {
        public DiscountMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_discount_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Discounts");
            this.Property(t => t.DBKEY_discount_id).HasColumnName("DBKEY_discount_id");
            this.Property(t => t.active).HasColumnName("active");
            this.Property(t => t.application_type).HasColumnName("application_type");
            this.Property(t => t.apply_to_base_product_only).HasColumnName("apply_to_base_product_only");
            this.Property(t => t.apply_to_entire_application_type).HasColumnName("apply_to_entire_application_type");
            this.Property(t => t.auto_apply).HasColumnName("auto_apply");
            this.Property(t => t.barcode).HasColumnName("barcode");
            this.Property(t => t.brand_level).HasColumnName("brand_level");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.discount_amount).HasColumnName("discount_amount");
            this.Property(t => t.discount_at_item_level).HasColumnName("discount_at_item_level");
            this.Property(t => t.discount_code).HasColumnName("discount_code");
            this.Property(t => t.discount_type).HasColumnName("discount_type");
            this.Property(t => t.display_on_ipad).HasColumnName("display_on_ipad");
            this.Property(t => t.effective_from).HasColumnName("effective_from");
            this.Property(t => t.establishment).HasColumnName("establishment");
            this.Property(t => t.how_often_apply).HasColumnName("how_often_apply");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.lock_enable).HasColumnName("lock_enable");
            this.Property(t => t.lock_uuid).HasColumnName("lock_uuid");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.old_taxed_flag).HasColumnName("old_taxed_flag");
            this.Property(t => t.password_required).HasColumnName("password_required");
            this.Property(t => t.qualification_subtype).HasColumnName("qualification_subtype");
            this.Property(t => t.qualification_type).HasColumnName("qualification_type");
            this.Property(t => t.resource_uri).HasColumnName("resource_uri");
            this.Property(t => t.taxed).HasColumnName("taxed");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
            this.Property(t => t.discount_id).HasColumnName("discount_id");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
            this.Property(t => t.effective_to).HasColumnName("effective_to");
            this.Property(t => t.maximum_off).HasColumnName("maximum_off");
            this.Property(t => t.minimum_amount).HasColumnName("minimum_amount");
        }
    }
}
