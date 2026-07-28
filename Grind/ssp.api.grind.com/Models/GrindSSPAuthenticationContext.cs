using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Web.Grind._808nd.com.Models;
using Web.Grind._808nd.com.Models.Mapping;

namespace ssp.api.grind.com.Models
{
    public class GrindSSPAuthenticationContext : DbContext
    {

        static GrindSSPAuthenticationContext()
        {
            Database.SetInitializer<GrindSSPAuthenticationContext>(null);
        }

        public GrindSSPAuthenticationContext()
            : base("Name=DefaultConnection")
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
