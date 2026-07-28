using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class ScheduledTaskLogMap : EntityTypeConfiguration<ScheduledTaskLog>
    {
        public ScheduledTaskLogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("ScheduledTaskLogs");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.FireTime).HasColumnName("FireTime");
            this.Property(t => t.PreviousFireTime).HasColumnName("PreviousFireTime");
            this.Property(t => t.NextFireTime).HasColumnName("NextFireTime");
            this.Property(t => t.Detail).HasColumnName("Detail");
            this.Property(t => t.Result).HasColumnName("Result");
        }
    }
}
