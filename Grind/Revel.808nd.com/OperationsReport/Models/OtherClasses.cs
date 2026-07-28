using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.OperationsReport.Models
{



    public class XeroOperationsProducClassGroupContainer
    {
        public IList<XeroOperationsProducClassGroup> XeroOperationsProducClassGroups { get; set; }
        public SalesData SalesData { get; set; }

        public double GetTotalTax()
        {
            return XeroOperationsProducClassGroups.Sum(x => x.ProductMix.tax);

        }

        public decimal GetTotalTaxableSales()
        {
            return XeroOperationsProducClassGroups.Sum(x => x.ProductMix.taxable_sales);

        }

        public decimal GetTotalUnTaxableSales()
        {
            return XeroOperationsProducClassGroups.Sum(x => x.ProductMix.untaxable_sales);

        }

        public decimal GetTotalGrossSales()
        {
            return Convert.ToDecimal(XeroOperationsProducClassGroups.Sum(x => x.ProductMix.price));

        }

        public decimal? GetTotalVoidSales()
        {

            return XeroOperationsProducClassGroups.Sum(x => x.ProductMix.voids_amount_total);
        }

        public decimal? GetTotalItemDiscountSales()
        {

            return XeroOperationsProducClassGroups.Sum(x => x.ProductMix.discount);
        }

        public decimal? GetTotalOrderDiscountSales()
        {

            return XeroOperationsProducClassGroups.Sum(x => x.ProductMix.order_discount);
        }




    }

    public class XeroOperationsProducClassGroup
    {
        public string ParentCategoryName { get; set; }
        public ProductMixData ProductMix { get; set; }
    }


    public class ProductMixData
    {
        public string product_category { get; set; }
        public double tax { get; set; }
        public string parent_pclass { get; set; }
        public decimal exchanged_amount { get; set; }
        public string product_sku { get; set; }
        public decimal cost { get; set; }
        [BsonRepresentation(BsonType.Decimal128, AllowTruncation = true)]
        public decimal untaxable_sales { get; set; }
        public string n_comps { get; set; }
        public decimal gm { get; set; }
        public decimal total { get; set; }
        public decimal voids_amount { get; set; }
        public string product_class { get; set; }
        public decimal? percent_price { get; set; }
        public string n_items { get; set; }
        public decimal? gm_percent { get; set; }
        public decimal? crv_value_sales { get; set; }
        public decimal? comps_amount { get; set; }
        public string product_name { get; set; }
        public decimal taxable_sales { get; set; }
        public string n_voids { get; set; }
        [BsonRepresentation(BsonType.Decimal128, AllowTruncation = true)]
        public decimal price { get; set; }
        public string row_type { get; set; }
        public string product_subcategory { get; set; }
        public string parent_product_name { get; set; }
        public decimal discount { get; set; }
        public string product_barcode { get; set; }
        public decimal? order_discount { get; set; }
        public decimal? food_cost { get; set; }
        public decimal? avg_price { get; set; }
        public decimal? product_weight { get; set; }
        public decimal? voids_amount_total { get; set; }
        public string product_description { get; set; }
        public decimal? crv_value_tax { get; set; }
        public string msrp { get; set; }
    }

    public class VoidsData
    {
        public string reason { get; set; }
        public double amount { get; set; }
        public string qty { get; set; }
    }

    public class CustomPayments
    {
    }

    public class Vouchers
    {
    }

    public class CountedDiscounts
    {
    }

    public class SalesData
    {
        public string house_account_payable { get; set; }
        public string house_account_receivable { get; set; }
        public string gift_sales_payable { get; set; }
        public string gift_sales_receivable { get; set; }
        public string store_credit_sales_payable { get; set; }
        public string store_credit_sales_receivable { get; set; }
        public string lia_deposits_payable { get; set; }
        public string lia_deposits_receivable { get; set; }
        public string applied_deposits_payable { get; set; }
        public string donations { get; set; }
        public string due_amount_payable { get; set; }
        public string due_amount_receivable { get; set; }
        public string house_account_receivable_tips { get; set; }
        public string gift_sales_payable_tips { get; set; }
        public string store_credit_sales_payable_tips { get; set; }
        public string lia_deposits_payable_tips { get; set; }
        public string lia_deposits_receivable_tips { get; set; }
        public string taxable_sales { get; set; }
        public string nontaxable_sales { get; set; }
        public string total_sales { get; set; }
        public string gross_sales { get; set; }
        public string net_sales { get; set; }
        public string gift_sales { get; set; }
        public string gift_payments { get; set; }
        public string gift_liabilities { get; set; }
        public string store_credit_sales { get; set; }
        public string store_credit_payments { get; set; }
        public string store_credit_liabilities { get; set; }
        public string deposit_total { get; set; }
        public string deposit_payments { get; set; }
        public string deposit_liabilities { get; set; }
        public string payments_for_ha { get; set; }
        public string applied_ha { get; set; }
        public string house_account_liabilities { get; set; }
        public string liabilities { get; set; }
        public string liabilities_total { get; set; }
        public string total_payments { get; set; }
        public string net_account_for { get; set; }
        public string deposit_tips { get; set; }
        public string net_sales_taxed { get; set; }
        public string net_sales_untaxed { get; set; }
        public decimal? cash_total { get; set; }
        public decimal? cash_for_deposits { get; set; }
        public decimal? cash_for_sales { get; set; }
        public decimal? cash_qty { get; set; }
        public decimal? cash_refunds { get; set; }
        public decimal? cash_gratuity { get; set; }
        public decimal? cash_totals_with_refunds { get; set; }
        public decimal? cash_house_account { get; set; }
        public string credit_for_deposits { get; set; }
        public string credit_refunds { get; set; }
        public string credit_gratuity { get; set; }
        public string credit_house_account { get; set; }
        public decimal? credit_qty { get; set; }
        public string credit_total { get; set; }
        public string credit_totals_with_refunds { get; set; }
        public string credit_for_sales { get; set; }
        public string debit_total { get; set; }
        public string debit_for_sales { get; set; }
        public string debit_for_deposits { get; set; }
        public string debit_refunds { get; set; }
        public string debit_house_account { get; set; }
        public string debit_gratuity { get; set; }
        public string debit_totals_with_refunds { get; set; }
        public string debit_qty { get; set; }
        public string check_totals_with_refunds { get; set; }
        public string check_refunds { get; set; }
        public string check_house_account { get; set; }
        public string check_total { get; set; }
        public string check_qty { get; set; }
        public string check_for_sales { get; set; }
        public string check_for_deposits { get; set; }
        public string check_gratuity { get; set; }
        public string trade_house_account { get; set; }
        public string trade_gratuity { get; set; }
        public string trade_qty { get; set; }
        public string trade_total { get; set; }
        public string trade_for_sales { get; set; }
        public string trade_totals_with_refunds { get; set; }
        public string trade_for_deposits { get; set; }
        public string trade_refunds { get; set; }
        public string levelup_totals_with_refunds { get; set; }
        public string levelup_for_deposits { get; set; }
        public string levelup_house_account { get; set; }
        public string levelup_refunds { get; set; }
        public decimal? levelup_qty { get; set; }
        public string levelup_for_sales { get; set; }
        public string levelup_gratuity { get; set; }
        public string badge_for_sales { get; set; }
        public string badge_gratuity { get; set; }
        public string badge_refunds { get; set; }
        public string badge_qty { get; set; }
        public string badge_for_deposits { get; set; }
        public string badge_totals_with_refunds { get; set; }
        public string badge_house_account { get; set; }
        public string index_for_deposits { get; set; }
        public string index_gratuity { get; set; }
        public decimal? index_qty { get; set; }
        public string index_refunds { get; set; }
        public string index_house_account { get; set; }
        public string index_totals_with_refunds { get; set; }
        public string index_for_sales { get; set; }
        public string paypal_for_sales { get; set; }
        public string paypal_totals_with_refunds { get; set; }
        public string paypal_gratuity { get; set; }
        public string paypal_for_deposits { get; set; }
        public string paypal_refunds { get; set; }
        public string paypal_house_account { get; set; }
        public decimal? paypal_qty { get; set; }
        public string ebt_foodstamp_for_deposits { get; set; }
        public string ebt_foodstamp_totals_with_refunds { get; set; }
        public string ebt_foodstamp_total { get; set; }
        public string ebt_foodstamp_gratuity { get; set; }
        public string ebt_foodstamp_house_account { get; set; }
        public string ebt_foodstamp_for_sales { get; set; }
        public string ebt_foodstamp_refunds { get; set; }
        public decimal? ebt_foodstamp_qty { get; set; }
        public string bitcoin_totals_with_refunds { get; set; }
        public string bitcoin_for_sales { get; set; }
        public decimal? bitcoin_qty { get; set; }
        public string bitcoin_house_account { get; set; }
        public string bitcoin_for_deposits { get; set; }
        public string bitcoin_gratuity { get; set; }
        public string bitcoin_total { get; set; }
        public string bitcoin_refunds { get; set; }
        public string ebt_cash_totals_with_refunds { get; set; }
        public string ebt_cash_house_account { get; set; }
        public decimal? ebt_cash_qty { get; set; }
        public string ebt_cash_for_deposits { get; set; }
        public string ebt_cash_for_sales { get; set; }
        public string ebt_cash_gratuity { get; set; }
        public string ebt_cash_total { get; set; }
        public string ebt_cash_refunds { get; set; }
        public string zapper_totals_with_refunds { get; set; }
        public string zapper_refunds { get; set; }
        public string zapper_house_account { get; set; }
        public string zapper_for_deposits { get; set; }
        public string zapper_for_sales { get; set; }
        public decimal? zapper_qty { get; set; }
        public string zapper_gratuity { get; set; }
        public string ssmc_house_account { get; set; }
        public string ssmc_totals_with_refunds { get; set; }
        public string ssmc_gratuity { get; set; }
        public decimal? ssmc_qty { get; set; }
        public string ssmc_for_sales { get; set; }
        public string ssmc_refunds { get; set; }
        public string ssmc_for_deposits { get; set; }
        public string sale_loss_drive_off_total { get; set; }
        public decimal? sale_loss_drive_off_qty { get; set; }
        public string sale_loss_drive_off_for_deposits { get; set; }
        public string sale_loss_drive_off_for_sales { get; set; }
        public string sale_loss_drive_off_refunds { get; set; }
        public string sale_loss_drive_off_house_account { get; set; }
        public string sale_loss_drive_off_totals_with_refunds { get; set; }
        public string sale_loss_drive_off_gratuity { get; set; }
        public string fleet_totals_with_refunds { get; set; }
        public string fleet_for_sales { get; set; }
        public string fleet_for_deposits { get; set; }
        public string fleet_gratuity { get; set; }
        public decimal? fleet_qty { get; set; }
        public string fleet_refunds { get; set; }
        public string fleet_house_account { get; set; }
        public string worapay_for_sales { get; set; }
        public string worapay_gratuity { get; set; }
        public string worapay_refunds { get; set; }
        public string worapay_for_deposits { get; set; }
        public string worapay_totals_with_refunds { get; set; }
        public string worapay_house_account { get; set; }
        public decimal? worapay_qty { get; set; }
        public string other_for_sales { get; set; }
        public decimal? other_qty { get; set; }
        public string other_refunds { get; set; }
        public string other_house_account { get; set; }
        public string other_totals_with_refunds { get; set; }
        public string other_gratuity { get; set; }
        public string other_for_deposits { get; set; }
        public string other_total { get; set; }
        public string como_for_sales { get; set; }
        public string como_refunds { get; set; }
        public string como_totals_with_refunds { get; set; }
        public string como_house_account { get; set; }
        public string como_gratuity { get; set; }
        public string como_for_deposits { get; set; }
        public decimal? como_qty { get; set; }
        public string plant_house_account { get; set; }
        public string plant_for_sales { get; set; }
        public string plant_refunds { get; set; }
        public decimal? plant_qty { get; set; }
        public string plant_gratuity { get; set; }
        public string plant_for_deposits { get; set; }
        public string plant_totals_with_refunds { get; set; }
        public string voucher_for_deposits { get; set; }
        public string voucher_totals_with_refunds { get; set; }
        public string voucher_gratuity { get; set; }
        public string voucher_refunds { get; set; }
        public string voucher_for_sales { get; set; }
        public decimal? voucher_qty { get; set; }
        public string voucher_house_account { get; set; }
        public string custom_payment_total { get; set; }
        public string custom_payment_for_deposits { get; set; }
        public string custom_payment_gratuity { get; set; }
        public string custom_payment_refunds { get; set; }
        public decimal? custom_payment_qty { get; set; }
        public string custom_payment_for_sales { get; set; }
        public string custom_payment_house_account { get; set; }
        public string custom_payment_totals_with_refunds { get; set; }
        public string deposit_raw { get; set; }
        public string payments_refund_total { get; set; }
        public string payments_total_for_sales { get; set; }
        public string payments_total_qty { get; set; }
        public string house_account_house_account { get; set; }
        public string american_express_qty { get; set; }
        public string american_express_refunds { get; set; }
        public string american_express_for_sales { get; set; }
        public string american_express_for_deposits { get; set; }
        public string american_express_house_account { get; set; }
        public string american_express_totals_with_refunds { get; set; }
        public string discover_qty { get; set; }
        public string discover_refunds { get; set; }
        public string discover_for_sales { get; set; }
        public string discover_for_deposits { get; set; }
        public string discover_house_account { get; set; }
        public string discover_totals_with_refunds { get; set; }
        public string mastercard_qty { get; set; }
        public string mastercard_refunds { get; set; }
        public string mastercard_for_sales { get; set; }
        public string mastercard_for_deposits { get; set; }
        public string mastercard_house_account { get; set; }
        public string mastercard_totals_with_refunds { get; set; }
        public string visa_qty { get; set; }
        public string visa_refunds { get; set; }
        public string visa_for_sales { get; set; }
        public string visa_for_deposits { get; set; }
        public string visa_house_account { get; set; }
        public string visa_totals_with_refunds { get; set; }
        public string maestro_qty { get; set; }
        public string maestro_refunds { get; set; }
        public string maestro_for_sales { get; set; }
        public string maestro_for_deposits { get; set; }
        public string maestro_house_account { get; set; }
        public string maestro_totals_with_refunds { get; set; }
        public string eftpos_qty { get; set; }
        public string eftpos_refunds { get; set; }
        public string eftpos_for_sales { get; set; }
        public string eftpos_for_deposits { get; set; }
        public string eftpos_house_account { get; set; }
        public string eftpos_totals_with_refunds { get; set; }
        public string other_credit_card_qty { get; set; }
        public string other_credit_card_refunds { get; set; }
        public string other_credit_card_for_sales { get; set; }
        public string other_credit_card_for_deposits { get; set; }
        public string other_credit_card_house_account { get; set; }
        public string other_credit_card_totals_with_refunds { get; set; }
        public string nets_card_qty { get; set; }
        public string nets_card_refunds { get; set; }
        public string nets_card_for_sales { get; set; }
        public string nets_card_for_deposits { get; set; }
        public string nets_card_house_account { get; set; }
        public string nets_card_totals_with_refunds { get; set; }
        public string cash_drops { get; set; }
        public string cash_drops_count { get; set; }
        public string expected_cash { get; set; }
        public string starting_cash { get; set; }
        public string actual_cash { get; set; }
        public string pure_discount_totals { get; set; }
        public string rounding_deltas { get; set; }
        public string togo_sales { get; set; }
        public string togo_sales_p { get; set; }
        public string eatin_sales { get; set; }
        public string eatin_sales_p { get; set; }
        public string delivery_sales { get; set; }
        public string delivery_sales_p { get; set; }
        public string catering_sales { get; set; }
        public string catering_sales_p { get; set; }
        public string drivethrough_sales { get; set; }
        public string drivethrough_sales_p { get; set; }
        public string online_sales { get; set; }
        public string online_sales_p { get; set; }
        public string other_sales { get; set; }
        public string other_sales_p { get; set; }
        public string shipping_sales { get; set; }
        public string shipping_sales_p { get; set; }
        public string service_fee_total { get; set; }
        public string taxable_service_fee { get; set; }
        public string nontaxable_service_fee { get; set; }
        public string ship_and_handling { get; set; }
        public string item_discounts { get; set; }
        public string order_discounts_total { get; set; }
        public string total_discounts { get; set; }
        public string sales_tax { get; set; }
        public string surcharges_total { get; set; }
        public string total_tax_surcharge_service { get; set; }
        public string crv_value_as_price_part { get; set; }
        public string crv_value_as_tax { get; set; }
        public string visa_total { get; set; }
        public string mastercard_total { get; set; }
        public string american_express_total { get; set; }
        public string maestro_total { get; set; }
        public string discover_total { get; set; }
        public string other_credit_card_total { get; set; }
        public string applied_deposit_total { get; set; }
        public string applied_deposit_for_sales { get; set; }
        public string applied_deposit_for_deposits { get; set; }
        public string applied_deposit_refunds { get; set; }
        public string applied_deposit_tips_total { get; set; }
        public string applied_deposit_totals_with_refunds { get; set; }
        public string applied_deposit_qty { get; set; }
        public string applied_deposit_other_credit_card { get; set; }
        public string applied_deposit_other_credit_card_tips { get; set; }
        public string applied_deposit_american_express { get; set; }
        public string applied_deposit_american_express_tips { get; set; }
        public string applied_deposit_discover { get; set; }
        public string applied_deposit_discover_tips { get; set; }
        public string applied_deposit_mastercard { get; set; }
        public string applied_deposit_mastercard_tips { get; set; }
        public string applied_deposit_visa { get; set; }
        public string applied_deposit_visa_tips { get; set; }
        public string applied_deposit_maestro { get; set; }
        public string applied_deposit_maestro_tips { get; set; }
        public string trade { get; set; }
        public string expected_total_cash_to_business { get; set; }
        public string actual_total_cash_to_business { get; set; }
        public string cash_tips_total { get; set; }
        public string credit_tips_total { get; set; }
        public string visa_tips { get; set; }
        public string mastercard_tips { get; set; }
        public string american_express_tips { get; set; }
        public string amex_tips { get; set; }
        public string maestro_tips { get; set; }
        public string discover_tips { get; set; }
        public string other_credit_card_tips { get; set; }
        public string paypal_tips_total { get; set; }
        public string custom_payment_tips_total { get; set; }
        public string tips_total { get; set; }
        public string payable_tips { get; set; }
        public string liabilities_tips_total { get; set; }
        public string payments_accepted_amount { get; set; }
        public string payments_captured_amount { get; set; }
        public string payments_declined_amount { get; set; }
        public string payments_failed_amount { get; set; }
        public string payments_pending_amount { get; set; }
        public string payments_unknown_amount { get; set; }
        public string other_tips_total { get; set; }
        public string refunds_total { get; set; }
        public string refunds_count { get; set; }
        public string payouts_count { get; set; }
        public string payouts_ins_total { get; set; }
        public string payouts_ins_count { get; set; }
        public string payouts_outs_total { get; set; }
        public string payouts_outs_count { get; set; }
        public string declared_tips { get; set; }
        public string safe_drops { get; set; }
        public string voided_total { get; set; }
        public string voided_items { get; set; }
        public string returned_total { get; set; }
        public string returned_items { get; set; }
        public string comps_total { get; set; }
        public string comps_items { get; set; }
        public string exchanged_total { get; set; }
        public string exchanged_items { get; set; }
        public string total_number_of_people { get; set; }
        public string avg_sale_per_person { get; set; }
        public string adj_credit_tips_total { get; set; }
        public string adj_credit_tips { get; set; }
        public string adj_visa_tips { get; set; }
        public string adj_visa_tips_total { get; set; }
        public string adj_maestro_tips { get; set; }
        public string adj_maestro_tips_total { get; set; }
        public string adj_mastercard_tips { get; set; }
        public string adj_mastercard_tips_total { get; set; }
        public string adj_american_express_tips { get; set; }
        public string adj_amex_tips { get; set; }
        public string adj_american_express_tips_total { get; set; }
        public string adj_amex_tips_total { get; set; }
        public string adj_other_credit_card_tips_total { get; set; }
        public string adj_other_credit_card_tips { get; set; }
        public string adj_paypal_tips { get; set; }
        public string adj_paypal_tips_total { get; set; }
        public string adj_discover_tips { get; set; }
        public string adj_discover_tips_total { get; set; }
        public string adj_non_cash_tips_total_minus_cash { get; set; }
        public string adj_tips_total { get; set; }
        public string adj_declined_tips_total { get; set; }
        public string adj_total { get; set; }
        public string total_invoices { get; set; }
        public decimal? total_orders { get; set; }
        public string avg_sale { get; set; }
        public string non_cash_tips_total_minus_cash { get; set; }
        public bool show_donations { get; set; }
        public string non_cash_tips { get; set; }
        public string adj_non_cash_tips { get; set; }
        public string cash_due_payments { get; set; }
        public CustomPayments custom_payments { get; set; }
        public Vouchers vouchers { get; set; }
        public CountedDiscounts counted_discounts { get; set; }
    }

    public class DiscountsData
    {
        public string reason { get; set; }
        public double amount { get; set; }
        public decimal? qty { get; set; }
    }

    public class TaxData
    {
        public string name { get; set; }
        public double taxable_sales { get; set; }
        public double tax { get; set; }
        public double sales { get; set; }
        public string verbose_tax_rate { get; set; }
        public double order_discounts { get; set; }
        public string tax_rate { get; set; }
        public double item_discounts { get; set; }
        public string local_tax_id { get; set; }
        public string row_type { get; set; }
    }

    public class RootObject
    {
        //public List<object> revenue_centers { get; set; }
        public List<ProductMixData> product_mix_data { get; set; }
        public List<VoidsData> voids_data { get; set; }
        public List<List<object>> employees { get; set; }
        public SalesData sales_data { get; set; }
        //public List<List<object>> posstations { get; set; }
        //public List<object> labor_data { get; set; }
        public List<DiscountsData> discounts_data { get; set; }
        public List<TaxData> tax_data { get; set; }
    }


    public class OpsReportHourlyWrapper
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string _id { get; set; } //primary key
        public RootObject opsReport { get; set; } //the actual data
        public int establishmentId { get; set; } //establishment
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime containerStart { get; set; } //when the container data query begins
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime containerEnd { get; set; } //when the container data query begins

    }

    public class DateTimeStartEndRange
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Start { get; set; } //when the container data query begins
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime End { get; set; } //when the container data query begins

    }

}
