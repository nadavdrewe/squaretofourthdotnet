namespace Revel._808nd.com.CaternetData.Models
{
    public class Entry
    {
        public string PLU { get; set; }
        public int Quantity { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal NetSalesPrice { get; set; }
        public decimal GrossSalesPrice { get; set; }
        public decimal TotalNetSales { get; set; }
        public decimal TotalGrossSales { get; set; }
        public string SalesTypeRef { get; set; }
        public string Notes { get; set; }
    }
}