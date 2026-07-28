using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class RewardsCardNewMap : EntityTypeConfiguration<RewardsCardNew>
    {
        public RewardsCardNewMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_rewardscardnew_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RewardsCardNews");
            this.Property(t => t.DBKEY_rewardscardnew_id).HasColumnName("DBKEY_rewardscardnew_id");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.current_points).HasColumnName("current_points");
            this.Property(t => t.establishment).HasColumnName("establishment");
            this.Property(t => t.number).HasColumnName("number");
            this.Property(t => t.payment_type).HasColumnName("payment_type");
            this.Property(t => t.resource_uri).HasColumnName("resource_uri");
            this.Property(t => t.total_points).HasColumnName("total_points");
            this.Property(t => t.total_purchases).HasColumnName("total_purchases");
            this.Property(t => t.total_visits).HasColumnName("total_visits");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
            this.Property(t => t.customer_id).HasColumnName("customer_id");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
            this.Property(t => t.theAddress).HasColumnName("theAddress");
            this.Property(t => t.is_vip_card).HasColumnName("is_vip_card");
            this.Property(t => t.ResourceUri).HasColumnName("ResourceUri");
            this.Property(t => t.Revelid).HasColumnName("Revelid");
            this.Property(t => t.vip_points_refresh).HasColumnName("vip_points_refresh");
            this.Property(t => t.customer_revel).HasColumnName("customer_revel");
            this.Property(t => t.notes).HasColumnName("notes");
            this.Property(t => t.days_since_last_visit).HasColumnName("days_since_last_visit");
            this.Property(t => t.yesterdaysTotalPoints).HasColumnName("yesterdaysTotalPoints");
            this.Property(t => t.yesterdaysTotalPointsWhenCreated).HasColumnName("yesterdaysTotalPointsWhenCreated");
            this.Property(t => t.pointsMultiplierLastRun).HasColumnName("pointsMultiplierLastRun");
            this.Property(t => t.vip_points_last_refreshed).HasColumnName("vip_points_last_refreshed");
        }
    }
}
