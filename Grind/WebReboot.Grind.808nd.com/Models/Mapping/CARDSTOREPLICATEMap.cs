using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class CARDSTOREPLICATEMap : EntityTypeConfiguration<CARDSTOREPLICATE>
    {
        public CARDSTOREPLICATEMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_rewardscardnew_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("CARDSTOREPLICATE");
            this.Property(t => t.DBKEY_rewardscardnew_id).HasColumnName("DBKEY_rewardscardnew_id");
        }
    }
}
