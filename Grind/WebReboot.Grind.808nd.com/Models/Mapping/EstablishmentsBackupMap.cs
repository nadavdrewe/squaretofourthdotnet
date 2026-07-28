using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class EstablishmentsBackupMap : EntityTypeConfiguration<EstablishmentsBackup>
    {
        public EstablishmentsBackupMap()
        {
            // Primary Key
            this.HasKey(t => t.establishment_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("EstablishmentsBackup");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
            this.Property(t => t.RevelOrganiationName).HasColumnName("RevelOrganiationName");
        }
    }
}
