using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Web.Grind._808nd.com.Models.Mapping;

namespace Web.Grind._808nd.com.Models
{
    public partial class GrindAuthContext : DbContext
    {
        static GrindAuthContext()
        {
            Database.SetInitializer<GrindAuthContext>(null);
        }

        public GrindAuthContext()
            : base("Name=GrindAuthContext")
        {
        }

        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AspNetRoleMap());
            modelBuilder.Configurations.Add(new AspNetUserClaimMap());
            modelBuilder.Configurations.Add(new AspNetUserLoginMap());
            modelBuilder.Configurations.Add(new AspNetUserMap());
        }
    }
}
