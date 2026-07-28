using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Web.Grind._808nd.com.Models.Mapping
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            this.HasKey(t => t.DBKEY_customer_id);

            // Properties
            this.Property(t => t.theAddress)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("Customers");
            this.Property(t => t.DBKEY_customer_id).HasColumnName("DBKEY_customer_id");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.CcExp).HasColumnName("CcExp");
            this.Property(t => t.CcFirstName).HasColumnName("CcFirstName");
            this.Property(t => t.CcLast4Digits).HasColumnName("CcLast4Digits");
            this.Property(t => t.CcLastName).HasColumnName("CcLastName");
            this.Property(t => t.CcNumber).HasColumnName("CcNumber");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.ExpDate).HasColumnName("ExpDate");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.IsVisitor).HasColumnName("IsVisitor");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.LicNumber).HasColumnName("LicNumber");
            this.Property(t => t.LoyaltyNumber).HasColumnName("LoyaltyNumber");
            this.Property(t => t.LoyaltyRefId).HasColumnName("LoyaltyRefId");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            this.Property(t => t.Picture).HasColumnName("Picture");
            this.Property(t => t.RefNumber).HasColumnName("RefNumber");
            this.Property(t => t.ResourceUri).HasColumnName("ResourceUri");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.TotalPurchases).HasColumnName("TotalPurchases");
            this.Property(t => t.TotalVisits).HasColumnName("TotalVisits");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.Uuid).HasColumnName("Uuid");
            this.Property(t => t.Zipcode).HasColumnName("Zipcode");
            this.Property(t => t.customer_id).HasColumnName("customer_id");
            this.Property(t => t.establishment_id).HasColumnName("establishment_id");
            this.Property(t => t.theAddress).HasColumnName("theAddress");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.RevelId).HasColumnName("RevelId");

            // Relationships
            this.HasRequired(t => t.Customer1)
                .WithOptional(t => t.Customers1);

        }
    }
}
