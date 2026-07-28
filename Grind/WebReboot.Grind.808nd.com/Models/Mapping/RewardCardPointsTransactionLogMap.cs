using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class RewardCardPointsTransactionLogMap : EntityTypeConfiguration<RewardCardPointsTransactionLog>
    {
        public RewardCardPointsTransactionLogMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RewardCardPointsTransactionLogs");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.orginal_points_total).HasColumnName("orginal_points_total");
            this.Property(t => t.new_points_total).HasColumnName("new_points_total");
            this.Property(t => t.pointsAdded).HasColumnName("pointsAdded");
            this.Property(t => t.multiplier).HasColumnName("multiplier");
            this.Property(t => t.card_number).HasColumnName("card_number");
            this.Property(t => t.orginal_points_current).HasColumnName("orginal_points_current");
            this.Property(t => t.new_points_current).HasColumnName("new_points_current");
            this.Property(t => t.WhenCreated).HasColumnName("WhenCreated");
        }
    }
}
