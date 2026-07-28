using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_address_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Addresses");
            this.Property(t => t.DBKEY_address_id).HasColumnName("DBKEY_address_id");
            this.Property(t => t.active).HasColumnName("active");
            this.Property(t => t.city).HasColumnName("city");
            this.Property(t => t.country).HasColumnName("country");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.customer_id).HasColumnName("customer_id");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.phone_number).HasColumnName("phone_number");
            this.Property(t => t.primary_billing).HasColumnName("primary_billing");
            this.Property(t => t.primary_shipping).HasColumnName("primary_shipping");
            this.Property(t => t.resource_uri).HasColumnName("resource_uri");
            this.Property(t => t.state).HasColumnName("state");
            this.Property(t => t.street_1).HasColumnName("street_1");
            this.Property(t => t.street_2).HasColumnName("street_2");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
            this.Property(t => t.uuid).HasColumnName("uuid");
            this.Property(t => t.zipcode).HasColumnName("zipcode");
            this.Property(t => t.Customer_DBKEY_customer_id).HasColumnName("Customer_DBKEY_customer_id");

            // Relationships
            this.HasOptional(t => t.Customer)
                .WithMany(t => t.Addresses)
                .HasForeignKey(d => d.Customer_DBKEY_customer_id);

        }
    }
}
