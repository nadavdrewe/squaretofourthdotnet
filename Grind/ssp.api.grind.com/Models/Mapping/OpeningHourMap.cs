using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class OpeningHourMap : EntityTypeConfiguration<OpeningHour>
    {
        public OpeningHourMap()
        {
            // Primary Key
            this.HasKey(t => t.OpeningHoursID);

            // Properties
            // Table & Column Mappings
            this.ToTable("OpeningHours");
            this.Property(t => t.OpeningHoursID).HasColumnName("OpeningHoursID");
            this.Property(t => t.Day).HasColumnName("Day");
            this.Property(t => t.OpeningTime).HasColumnName("OpeningTime");
            this.Property(t => t.ClosingTime).HasColumnName("ClosingTime");
            this.Property(t => t.Establishment_DBKEY_establishment_id).HasColumnName("Establishment_DBKEY_establishment_id");
        }
    }
}
