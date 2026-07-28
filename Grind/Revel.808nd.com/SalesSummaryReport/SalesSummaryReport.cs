using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.SalesSummaryReport
{
    public class CustomPayments
    {
    }

    public class Vouchers
    {
    }

    public class CountedDiscounts
    {
    }

    public class __invalid_type__4
    {
    }

    public class OnlineOrdersOptions
    {
        public __invalid_type__4 __invalid_name__4 { get; set; }
    }

    public class RootObjectSalesSummary
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
        public string gift_tips_total { get; set; }
        public string store_credit_sales { get; set; }
        public string store_credit_payments { get; set; }
        public string store_credit_liabilities { get; set; }
        public string deposit_total { get; set; }
        public string deposit_payments { get; set; }
        public string deposit_liabilities { get; set; }
        public string safe_drops_count { get; set; }
        public string safe_counts_count { get; set; }
        public string bank_deposits_count { get; set; }
        public string total_declared_safe_drops { get; set; }
        public string total_actual_safe_drops { get; set; }
        public string total_safe_counts { get; set; }
        public string total_bank_deposits { get; set; }
        public string safe_drop_variance { get; set; }
        public string amount_to_deposit { get; set; }
        public string amount_in_safe { get; set; }
        public string payments_for_ha { get; set; }
        public string applied_ha { get; set; }
        public string house_account_liabilities { get; set; }
        public string house_account_tips_total { get; set; }
        public string liabilities { get; set; }
        public string liabilities_total { get; set; }
        public string total_payments { get; set; }
        public string net_account_for { get; set; }
        public string deposit_tips { get; set; }
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
        public string cash_total { get; set; }
        public string credit_total { get; set; }
        public string visa_total { get; set; }
        public string mastercard_total { get; set; }
        public string american_express_total { get; set; }
        public string discover_total { get; set; }
        public string other_credit_card_total { get; set; }
        public string debit_total { get; set; }
        public string check_total { get; set; }
        public string gift_total { get; set; }
        public string applied_deposit_total { get; set; }
        public string applied_deposit_for_sales { get; set; }
        public string applied_deposit_for_deposits { get; set; }
        public string applied_deposit_refunds { get; set; }
        public string applied_deposit_tips_total { get; set; }
        public string applied_deposit_totals_with_refunds { get; set; }
        public int applied_deposit_qty { get; set; }
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
        public decimal other_total { get; set; }
        public string custom_payment_total { get; set; }
        public string expected_total_cash_to_business { get; set; }
        public string actual_total_cash_to_business { get; set; }
        public string cash_tips_total { get; set; }
        public string credit_tips_total { get; set; }
        public string visa_tips { get; set; }
        public string mastercard_tips { get; set; }
        public string american_express_tips { get; set; }
        public string amex_tips { get; set; }
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
        public double voided_items { get; set; }
        public string returned_total { get; set; }
        public double returned_items { get; set; }
        public string comps_total { get; set; }
        public double comps_items { get; set; }
        public string exchanged_total { get; set; }
        public double exchanged_items { get; set; }
        public double total_number_of_people { get; set; }
        public string avg_sale_per_person { get; set; }
        public string adj_credit_tips_total { get; set; }
        public string adj_credit_tips { get; set; }
        public string adj_visa_tips { get; set; }
        public string adj_visa_tips_total { get; set; }
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
        public double total_invoices { get; set; }
        public double total_orders { get; set; }
        public string avg_sale { get; set; }
        public string non_cash_tips_total_minus_cash { get; set; }
        public bool show_donations { get; set; }
        public string non_cash_tips { get; set; }
        public string adj_non_cash_tips { get; set; }
        public string cash_due_payments { get; set; }
        //public CustomPayments custom_payments { get; set; }
        //public Vouchers vouchers { get; set; }
        //public CountedDiscounts counted_discounts { get; set; }
        //public List<object> payments { get; set; }
        //public List<object> error_payments { get; set; }
        //public object percentage_row { get; set; }
        //public List<List<object>> sales_per_period { get; set; }
        //public List<List<object>> graph_data { get; set; }
        //public List<int> out_of_range { get; set; }
        //public List<int> invalid { get; set; }
        //public OnlineOrdersOptions online_orders_options { get; set; }
        //public List<List<object>> posstations { get; set; }
        //public List<object> revenue_centers { get; set; }
        //public List<object> errors { get; set; }
        //public List<List<object>> employees { get; set; }
        //public int open_orders_count { get; set; }
        //public List<object> open_orders_ids { get; set; }
        //public List<object> unpaid_orders_ids { get; set; }
        //public bool has_any_filters { get; set; }
        //public object include_tips { get; set; }
        //public string total_tax_breakdowns { get; set; }
        //public List<List<object>> tax_breakdowns { get; set; }
    }
}
