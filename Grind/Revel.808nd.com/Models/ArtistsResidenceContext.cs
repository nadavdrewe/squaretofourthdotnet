//using elina.railgunit.com.Model;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Revel._808nd.com.Models.WebhookModels;
using Revel._808nd.com.Models.WebhookModels.Order;
using System.Data.Entity;
using domain.artistresidence.railgunit.com;
using System.ComponentModel.DataAnnotations;
using System;

namespace domain.artistresidence.railgunit.com.DataContext
{
    public class ArtistsResidenceContext : RevelContext
    {

        public ArtistsResidenceContext()
             : base("Name=ArtistsResidenceContext")
        //: base("Name=ARTest")
        {
            this.Database.CommandTimeout = 0;
        }

        public virtual DbSet<ElinaToRevelOrderUpdateLog> ElinaToRevelOrderUpdateLogs { get; set; }
        public virtual DbSet<ChargeRoomLogForOtherSystem> ChargeRoomLogForOtherSystems { get; set; }
        public virtual DbSet<ElinaStore> ElinaStores { get; set; }
        public virtual DbSet<TimeSheetEntry> TimeSheetEntries { get; set; }
        //public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<OrderInfo> OrderInfoes { get; set; }
        public virtual DbSet<OrderInfoItem> OrderInfoItem { get; set; }
        public virtual DbSet<OrderInfoPayment> OrderInfoPayment { get; set; }
        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<CustomerGroupCustomer> CustomerGroupCustomers { get; set; }

        public virtual DbSet<ChargeRoomOrderItemLog> ChargeRoomOrderItemLogs { get; set; }
        public virtual DbSet<ChargeRoomOrderLog> ChargeRoomOrderLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasKey(x => x.DBKEY_customer_id);
            modelBuilder.Entity<RewardsCardNew>().HasKey(x => x.DBKEY_rewardscardnew_id);

            modelBuilder.Entity<OrderInfo>()
             .HasMany<OrderInfoItem>(c => c.Items).WithRequired(x => x.OrderInfo).WillCascadeOnDelete();

            modelBuilder.Entity<OrderInfo>()
             .HasMany<OrderInfoPayment>(c => c.Payments).WithRequired(x => x.OrderInfo).WillCascadeOnDelete();

            base.OnModelCreating(modelBuilder);


        }

    }

    public class ElinaToRevelOrderUpdateLog
    {
        [Key]
        public int Id { get; set; }
        public string RevelOrderId { get; set; }
        public string ReservationId { get; set; }
        public string Establishment { get; set; }
        public string TicketNumber { get; set; }

        public string InvoiceId { get; set; }
        public string PaymentId { get; set; }
        public decimal FinalTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime WhenCreated { get; set; }
    }

    public class ChargeRoomLogForOtherSystem
    {

        [Key]
        public int Id { get; set; }

        public string Establishment { get; set; }
        public string TicketNumber { get; set; }

        public string RevelOrderId { get; set; }
        public string ReservationId { get; set; }

        public decimal FinalTotal { get; set; }
        public DateTime WhenLoggedUTC { get; set; }

    }

    public class ElinaStore
    {
        [Key]
        public int Id { get; set; }
        public string LinkingId { get; set; }
        public string LinkingURI { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Endpoint { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
    }


}
