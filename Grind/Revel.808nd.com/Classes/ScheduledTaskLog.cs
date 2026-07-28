using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Revel._808nd.com.Classes
{
    public class ScheduledTaskLog
    {
        [Key]     
        public int Id { get; set; }
        public string Message { get; set; }
        public Nullable<DateTime> FireTime { get; set; }
        public Nullable<DateTime> ContainerStartDate { get; set; }
        public Nullable<DateTime> ContainerEndDate { get; set; }
        public string Detail { get; set; }
        public int Result { get; set; }
        public int Brand { get; set; }
        public string BrandName { get; set; }
        public int Establishment { get; set; }
        public string EstablishmentName { get; set; }
        public decimal TotalPounds { get; set; }
        public decimal TotalVAT { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalItemQuantity { get; set; }
        public int TotalItemDiscountCount { get; set; }
        public decimal TotalItemDiscountAmount { get; set; }
        public decimal TotalItemDiscountTax { get; set; }
        public int TotalItemVoidedCount { get; set; }
        public decimal TotalItemVoidedAmount { get; set; }
        public string LogType { get; set; }
        public string User { get; set; }
        public string Notes { get; set; }
    }
}
