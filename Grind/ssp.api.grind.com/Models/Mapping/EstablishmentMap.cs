using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class EstablishmentMap : EntityTypeConfiguration<Establishment>
    {
        public EstablishmentMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_establishment_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Establishments");
            this.Property(t => t.DBKEY_establishment_id).HasColumnName("DBKEY_establishment_id");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
            this.Property(t => t.RevelOrganiationName).HasColumnName("RevelOrganiationName");
        }
    }
}
