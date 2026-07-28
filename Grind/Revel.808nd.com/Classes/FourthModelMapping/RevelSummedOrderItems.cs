using System;

namespace Revel._808nd.com.Classes.FourthModelMapping
{
    public class RevelSummedOrderItems
    {
        public int DBKEY_ID { get; set; }
        public int BRAND_ID { get; set; }
        public int ESTABLISHMENT_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public string SKU { get; set; }
        public string NAME { get; set; }
        public int QUANTITY { get; set; }
        public decimal PRICE { get; set; }
        public decimal MODIFIER_AMOUNT { get; set; }
        public decimal GROSS_W_MODIFIERS { get; set; }
        public decimal GROSS_NO_MODIFIERS { get; set; }
        public decimal GROSS_MINUS_TAX { get; set; }
        public decimal PURE_SALES { get; set; }
        public decimal PURE_SALES_PLUS_TAX { get; set; }
        public decimal DISCOUNT { get; set; }
        public decimal TAX { get; set; }
        public string VOIDED_REASON { get; set; }
        public DateTime CREATED_DATE { get; set; }

    }
}
