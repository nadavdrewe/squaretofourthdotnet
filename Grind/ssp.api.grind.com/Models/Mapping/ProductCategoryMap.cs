using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_productcategory_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("ProductCategories");
            this.Property(t => t.DBKEY_productcategory_id).HasColumnName("DBKEY_productcategory_id");
            this.Property(t => t.establishment).HasColumnName("establishment");
            this.Property(t => t.productcategory_id).HasColumnName("productcategory_id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
        }
    }
}
