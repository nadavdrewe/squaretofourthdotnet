using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class RewardsCardDailyPointMap : EntityTypeConfiguration<RewardsCardDailyPoint>
    {
        public RewardsCardDailyPointMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RewardsCardDailyPoints");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.date).HasColumnName("date");
            this.Property(t => t.total_points_on_date).HasColumnName("total_points_on_date");
            this.Property(t => t.RewardsCardNew_DBKEY_rewardscardnew_id).HasColumnName("RewardsCardNew_DBKEY_rewardscardnew_id");
            this.Property(t => t.current_points_on_date).HasColumnName("current_points_on_date");
            this.Property(t => t.card_number).HasColumnName("card_number");

            // Relationships
            this.HasOptional(t => t.RewardsCardNew)
                .WithMany(t => t.RewardsCardDailyPoints)
                .HasForeignKey(d => d.RewardsCardNew_DBKEY_rewardscardnew_id);

        }
    }
}
