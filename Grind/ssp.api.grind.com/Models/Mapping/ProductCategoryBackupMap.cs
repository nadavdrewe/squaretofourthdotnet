using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class ProductCategoryBackupMap : EntityTypeConfiguration<ProductCategoryBackup>
    {
        public ProductCategoryBackupMap()
        {
            // Primary Key
            this.HasKey(t => new { t.productcategory_id, t.establishment_id });

            // Properties
            this.Property(t => t.productcategory_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.establishment_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ProductCategoryBackup");
            this.Property(t => t.productcategory_id).HasColumnName("productcategory_id");
            this.Property(t => t.establishment).HasColumnName("establishment");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
        }
    }
}
