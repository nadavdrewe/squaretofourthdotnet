using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class ProductsBackupMap : EntityTypeConfiguration<ProductsBackup>
    {
        public ProductsBackupMap()
        {
            // Primary Key
            this.HasKey(t => new { t.product_id, t.allow_price_override, t.attribute_type, t.color_code, t.cost, t.crv_enabled, t.deleted, t.disable_modifier_popup, t.display_on_kiosk, t.display_online, t.ebt_no, t.export, t.happy_hour, t.is_cold, t.is_combo, t.is_drink, t.lock_enable, t.max_price, t.preparation_time, t.price, t.price_embedded, t.product_weight_unit, t.rti_combo, t.sold_by_weight, t.sorting, t.tax_class, t.tax_included, t.variable_pricing, t.variable_pricing_by, t.establishment_id, t.categoryID });

            // Properties
            this.Property(t => t.product_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.attribute_type)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.color_code)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.cost)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.max_price)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.preparation_time)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.price)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.product_weight_unit)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.sorting)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.tax_class)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.variable_pricing_by)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.establishment_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.categoryID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ProductsBackup");
            this.Property(t => t.product_id).HasColumnName("product_id");
            this.Property(t => t.active).HasColumnName("active");
            this.Property(t => t.allow_price_override).HasColumnName("allow_price_override");
            this.Property(t => t.attribute_type).HasColumnName("attribute_type");
            this.Property(t => t.barcode).HasColumnName("barcode");
            this.Property(t => t.brand).HasColumnName("brand");
            this.Property(t => t.category).HasColumnName("category");
            this.Property(t => t.color_code).HasColumnName("color_code");
            this.Property(t => t.commission).HasColumnName("commission");
            this.Property(t => t.cost).HasColumnName("cost");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.crv_enabled).HasColumnName("crv_enabled");
            this.Property(t => t.deleted).HasColumnName("deleted");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.dining_options).HasColumnName("dining_options");
            this.Property(t => t.disable_modifier_popup).HasColumnName("disable_modifier_popup");
            this.Property(t => t.display_on_kiosk).HasColumnName("display_on_kiosk");
            this.Property(t => t.display_online).HasColumnName("display_online");
            this.Property(t => t.ebt_no).HasColumnName("ebt_no");
            this.Property(t => t.establishment).HasColumnName("establishment");
            this.Property(t => t.export).HasColumnName("export");
            this.Property(t => t.happy_hour).HasColumnName("happy_hour");
            this.Property(t => t.is_cold).HasColumnName("is_cold");
            this.Property(t => t.is_combo).HasColumnName("is_combo");
            this.Property(t => t.is_drink).HasColumnName("is_drink");
            this.Property(t => t.kitchen_print_name).HasColumnName("kitchen_print_name");
            this.Property(t => t.lock_enable).HasColumnName("lock_enable");
            this.Property(t => t.max_price).HasColumnName("max_price");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.preparation_time).HasColumnName("preparation_time");
            this.Property(t => t.price).HasColumnName("price");
            this.Property(t => t.price_embedded).HasColumnName("price_embedded");
            this.Property(t => t.product_weight_unit).HasColumnName("product_weight_unit");
            this.Property(t => t.productclass).HasColumnName("productclass");
            this.Property(t => t.resource_uri).HasColumnName("resource_uri");
            this.Property(t => t.rti_combo).HasColumnName("rti_combo");
            this.Property(t => t.sku).HasColumnName("sku");
            this.Property(t => t.sold_by_weight).HasColumnName("sold_by_weight");
            this.Property(t => t.sorting).HasColumnName("sorting");
            this.Property(t => t.tare).HasColumnName("tare");
            this.Property(t => t.tax).HasColumnName("tax");
            this.Property(t => t.tax_class).HasColumnName("tax_class");
            this.Property(t => t.tax_included).HasColumnName("tax_included");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
            this.Property(t => t.uuid).HasColumnName("uuid");
            this.Property(t => t.variable_pricing).HasColumnName("variable_pricing");
            this.Property(t => t.variable_pricing_by).HasColumnName("variable_pricing_by");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
            this.Property(t => t.productclass_id).HasColumnName("productclass_id");
            this.Property(t => t.tax_id).HasColumnName("tax_id");
            this.Property(t => t.brand_id).HasColumnName("brand_id");
            this.Property(t => t.categoryID).HasColumnName("categoryID");
        }
    }
}
