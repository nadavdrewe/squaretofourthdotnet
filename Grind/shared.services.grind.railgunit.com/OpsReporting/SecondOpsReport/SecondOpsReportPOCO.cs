using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.services.grind.railgunit.com.OpsReporting.SecondOpsReport
{
    public class VATRATEReportPOCO {

        public int EstablishmentId { get; set; }
        public string EstablishmentName { get; set; }

        //VAT RATE FIELDS
        public string _20PercentVAT_TaxableSales { get; set; }
        public string _20PercentVAT_Tax { get; set; }
        public string _20PercentVAT_TaxRate { get; set; }

        public string _5PercentVAT_TaxableSales { get; set; }
        public string _5PercentVAT_Tax { get; set; }
        public string _5PercentVAT_TaxRate { get; set; }

        public string _5PercentVATZeroRate_TaxableSales { get; set; }
        public string _5PercentVATZeroRate_Tax { get; set; }
        public string _5PercentVATZeroRate_TaxRate { get; set; }

        public string _Untaxed_TaxableSales { get; set; }
        public string _Untaxed_Tax { get; set; }
        public string _Untaxed_TaxRate { get; set; }

        public string _PrevailingTax_TaxableSales { get; set; }
        public string _PrevailingTax_Tax { get; set; }
        public string _PrevailingTax_TaxRate { get; set; }

        
        //PREVIOUS FIELDS
        public decimal Discounts { get; set; }

        public decimal Untaxed_Service_Fee { get; set; }

        public decimal Tips { get; set; }
        public decimal VAT { get; set; }
        public decimal House_Account { get; set; }
        public decimal Gift_Card_Purchases { get; set; }
        public decimal Gift_Cards_Used { get; set; }
        public decimal Variance { get; set; }
        public decimal Net_to_Account_For { get; set; }

        public decimal Payments { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }
        public decimal American_Express { get; set; }
        public decimal MasterCard { get; set; }
        public decimal Visa { get; set; }
        public decimal App { get; set; }
        public decimal OtherCredit { get; set; }
        public decimal Custom_Payment { get; set; }
        public decimal Grand_Total { get; set; }

        public decimal Payins { get; set; }
        public decimal Payouts { get; set; }

    }




    public class SecondOpsReportPOCO
    {
        public int EstablishmentId { get; set; }
        public string EstablishmentName { get; set; }

        public decimal Bar { get; set; }
        public decimal BarTaxable { get; set; }
        public decimal BarUntaxed { get; set; }

        public decimal Coffee_Hot_Drinks { get; set; }
        public decimal Coffee_Hot_DrinksTaxable { get; set; }
        public decimal Coffee_Hot_DrinksUntaxed { get; set; }

        public decimal Food { get; set; }
        public decimal FoodTaxable { get; set; }
        public decimal FoodUntaxed { get; set; }

        //public decimal Juice { get; set; }
        //public decimal JuiceTaxable { get; set; }
        //public decimal JuiceUntaxed { get; set; }

        //public decimal Soft_Drinks { get; set; }
        //public decimal Soft_DrinksTaxable { get; set; }
        // public decimal Soft_DrinksUntaxed { get; set; }

        public decimal Extra_Items { get; set; }
        public decimal Extra_ItemsTaxable { get; set; }
        public decimal Extra_ItemsUntaxed { get; set; }

        public decimal Retail { get; set; }
        public decimal RetailTaxable { get; set; }
        public decimal RetailUntaxed { get; set; }

        public decimal Unknown_Class { get; set; }
        public decimal Unknown_ClassTaxable { get; set; }
        public decimal Unknown_ClassUntaxed { get; set; }

        public decimal Discounts { get; set; }

        public decimal Untaxed_Service_Fee { get; set; }

        public decimal Tips { get; set; }
        public decimal VAT { get; set; }
        public decimal House_Account { get; set; }
        public decimal Gift_Card_Purchases { get; set; }
        public decimal Gift_Cards_Used { get; set; }
        public decimal Variance { get; set; }
        public decimal Net_to_Account_For { get; set; }

        public decimal Payments { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }
        public decimal American_Express { get; set; }
        public decimal MasterCard { get; set; }
        public decimal Visa { get; set; }
        public decimal App { get; set; }
        public decimal OtherCredit { get; set; }
        public decimal Custom_Payment { get; set; }
        public decimal Grand_Total { get; set; }

        public decimal Payins { get; set; }
        public decimal Payouts { get; set; }

    }

    
}
