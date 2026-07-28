using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class RewardsPointsMultiplierMap : EntityTypeConfiguration<RewardsPointsMultiplier>
    {
        public RewardsPointsMultiplierMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RewardsPointsMultipliers");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emailSuffix).HasColumnName("emailSuffix");
            this.Property(t => t.multiplier).HasColumnName("multiplier");
            this.Property(t => t.active).HasColumnName("active");
        }
    }
}
