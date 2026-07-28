using System.Text.Json;
using System.Text.Json.Serialization;

namespace RevelFourthPipeline.Domain.Revel;

public sealed class OperationsReport
{
    [JsonPropertyName("product_mix_data")]
    public List<ProductMixData> ProductMixData { get; set; } = [];

    [JsonPropertyName("voids_data")]
    public List<VoidData> VoidsData { get; set; } = [];

    [JsonPropertyName("employees")]
    public List<List<JsonElement>> Employees { get; set; } = [];

    [JsonPropertyName("sales_data")]
    public SalesData SalesData { get; set; } = new();

    [JsonPropertyName("discounts_data")]
    public List<DiscountData> DiscountsData { get; set; } = [];

    [JsonPropertyName("tax_data")]
    public List<TaxData> TaxData { get; set; } = [];

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class ProductMixData
{
    [JsonPropertyName("product_category")]
    public string? ProductCategory { get; set; }

    [JsonPropertyName("tax")]
    public decimal Tax { get; set; }

    [JsonPropertyName("parent_pclass")]
    public string? ParentProductClass { get; set; }

    [JsonPropertyName("exchanged_amount")]
    public decimal ExchangedAmount { get; set; }

    [JsonPropertyName("product_sku")]
    public string? ProductSku { get; set; }

    [JsonPropertyName("cost")]
    public decimal Cost { get; set; }

    [JsonPropertyName("untaxable_sales")]
    public decimal UntaxableSales { get; set; }

    [JsonPropertyName("n_comps")]
    public string? NumberOfComps { get; set; }

    [JsonPropertyName("gm")]
    public decimal GrossMargin { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("voids_amount")]
    public decimal VoidsAmount { get; set; }

    [JsonPropertyName("product_class")]
    public string? ProductClass { get; set; }

    [JsonPropertyName("percent_price")]
    public decimal? PercentPrice { get; set; }

    [JsonPropertyName("n_items")]
    public string? NumberOfItems { get; set; }

    [JsonPropertyName("gm_percent")]
    public decimal? GrossMarginPercent { get; set; }

    [JsonPropertyName("crv_value_sales")]
    public decimal? CrvValueSales { get; set; }

    [JsonPropertyName("comps_amount")]
    public decimal? CompsAmount { get; set; }

    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("taxable_sales")]
    public decimal TaxableSales { get; set; }

    [JsonPropertyName("n_voids")]
    public string? NumberOfVoids { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("row_type")]
    public string? RowType { get; set; }

    [JsonPropertyName("product_subcategory")]
    public string? ProductSubcategory { get; set; }

    [JsonPropertyName("parent_product_name")]
    public string? ParentProductName { get; set; }

    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    [JsonPropertyName("product_barcode")]
    public string? ProductBarcode { get; set; }

    [JsonPropertyName("order_discount")]
    public decimal? OrderDiscount { get; set; }

    [JsonPropertyName("food_cost")]
    public decimal? FoodCost { get; set; }

    [JsonPropertyName("avg_price")]
    public decimal? AveragePrice { get; set; }

    [JsonPropertyName("product_weight")]
    public decimal? ProductWeight { get; set; }

    [JsonPropertyName("voids_amount_total")]
    public decimal? VoidsAmountTotal { get; set; }

    [JsonPropertyName("product_description")]
    public string? ProductDescription { get; set; }

    [JsonPropertyName("crv_value_tax")]
    public decimal? CrvValueTax { get; set; }

    [JsonPropertyName("msrp")]
    public string? Msrp { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class VoidData
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("qty")]
    public string? Quantity { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class DiscountData
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("qty")]
    public decimal? Quantity { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class TaxData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("taxable_sales")]
    public decimal TaxableSales { get; set; }

    [JsonPropertyName("tax")]
    public decimal Tax { get; set; }

    [JsonPropertyName("sales")]
    public decimal Sales { get; set; }

    [JsonPropertyName("verbose_tax_rate")]
    public string? VerboseTaxRate { get; set; }

    [JsonPropertyName("order_discounts")]
    public decimal OrderDiscounts { get; set; }

    [JsonPropertyName("tax_rate")]
    public string? TaxRate { get; set; }

    [JsonPropertyName("item_discounts")]
    public decimal ItemDiscounts { get; set; }

    [JsonPropertyName("local_tax_id")]
    public string? LocalTaxId { get; set; }

    [JsonPropertyName("row_type")]
    public string? RowType { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class SalesData
{
    [JsonPropertyName("taxable_sales")]
    public string? TaxableSales { get; set; }

    [JsonPropertyName("nontaxable_sales")]
    public string? NonTaxableSales { get; set; }

    [JsonPropertyName("total_sales")]
    public string? TotalSales { get; set; }

    [JsonPropertyName("gross_sales")]
    public string? GrossSales { get; set; }

    [JsonPropertyName("net_sales")]
    public string? NetSales { get; set; }

    [JsonPropertyName("net_sales_taxed")]
    public string? NetSalesTaxed { get; set; }

    [JsonPropertyName("net_sales_untaxed")]
    public string? NetSalesUntaxed { get; set; }

    [JsonPropertyName("sales_tax")]
    public string? SalesTax { get; set; }

    [JsonPropertyName("item_discounts")]
    public string? ItemDiscounts { get; set; }

    [JsonPropertyName("order_discounts_total")]
    public string? OrderDiscountsTotal { get; set; }

    [JsonPropertyName("total_discounts")]
    public string? TotalDiscounts { get; set; }

    [JsonPropertyName("voided_total")]
    public string? VoidedTotal { get; set; }

    [JsonPropertyName("voided_items")]
    public string? VoidedItems { get; set; }

    [JsonPropertyName("returned_total")]
    public string? ReturnedTotal { get; set; }

    [JsonPropertyName("returned_items")]
    public string? ReturnedItems { get; set; }

    [JsonPropertyName("comps_total")]
    public string? CompsTotal { get; set; }

    [JsonPropertyName("comps_items")]
    public string? CompsItems { get; set; }

    [JsonPropertyName("total_orders")]
    public decimal? TotalOrders { get; set; }

    [JsonPropertyName("avg_sale")]
    public string? AverageSale { get; set; }

    [JsonPropertyName("total_number_of_people")]
    public string? TotalNumberOfPeople { get; set; }

    [JsonPropertyName("total_payments")]
    public string? TotalPayments { get; set; }

    [JsonPropertyName("payments_total_for_sales")]
    public string? PaymentsTotalForSales { get; set; }

    [JsonPropertyName("cash_total")]
    public decimal? CashTotal { get; set; }

    [JsonPropertyName("credit_total")]
    public string? CreditTotal { get; set; }

    [JsonPropertyName("visa_total")]
    public string? VisaTotal { get; set; }

    [JsonPropertyName("mastercard_total")]
    public string? MastercardTotal { get; set; }

    [JsonPropertyName("american_express_total")]
    public string? AmericanExpressTotal { get; set; }

    [JsonPropertyName("maestro_total")]
    public string? MaestroTotal { get; set; }

    [JsonPropertyName("other_credit_card_total")]
    public string? OtherCreditCardTotal { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class OperationsReportWrapper
{
    public string? Id { get; set; }
    public OperationsReport? OperationsReport { get; set; }
    public int EstablishmentId { get; set; }
    public DateTime ContainerStart { get; set; }
    public DateTime ContainerEnd { get; set; }
}

public sealed class DateTimeRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
