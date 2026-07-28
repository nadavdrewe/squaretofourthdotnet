using System.Text.Json;
using System.Text.Json.Serialization;

namespace RevelFourthPipeline.Domain.Revel;

public sealed class ProductMixReport
{
    [JsonPropertyName("productmix")]
    public List<ProductMixRow> ProductMix { get; set; } = [];

    [JsonPropertyName("product_fields")]
    public List<List<string>> ProductFields { get; set; } = [];

    [JsonPropertyName("product_classes")]
    public List<ProductMixProductClass> ProductClasses { get; set; } = [];

    [JsonPropertyName("categories")]
    public List<ProductMixCategory> Categories { get; set; } = [];

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class ProductMixProductClass
{
    [JsonPropertyName("parent_class_id")]
    public int ParentClassId { get; set; }

    [JsonPropertyName("parent_class_name")]
    public string? ParentClassName { get; set; }

    [JsonPropertyName("class_id")]
    public int ClassId { get; set; }

    [JsonPropertyName("class_name")]
    public string? ClassName { get; set; }
}

public sealed class ProductMixCategory
{
    [JsonPropertyName("sub_category_name")]
    public string? SubCategoryName { get; set; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("sub_category_id")]
    public int SubCategoryId { get; set; }

    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }
}

public sealed class ProductMixRow
{
    [JsonPropertyName("product_category")]
    public string? ProductCategory { get; set; }

    [JsonPropertyName("tax")]
    public decimal Tax { get; set; }

    [JsonPropertyName("parent_pclass")]
    public string? ParentProductClass { get; set; }

    [JsonPropertyName("product_sku")]
    public string? ProductSku { get; set; }

    [JsonPropertyName("cost")]
    public decimal? Cost { get; set; }

    [JsonPropertyName("untaxable_sales")]
    public decimal UntaxableSales { get; set; }

    [JsonPropertyName("n_comps")]
    public string? NumberOfComps { get; set; }

    [JsonPropertyName("gm")]
    public decimal? GrossMargin { get; set; }

    [JsonPropertyName("total")]
    public decimal? Total { get; set; }

    [JsonPropertyName("n_items")]
    public string? NumberOfItems { get; set; }

    [JsonPropertyName("n_voids")]
    public string? NumberOfVoids { get; set; }

    [JsonPropertyName("percent_price")]
    public decimal? PercentPrice { get; set; }

    [JsonPropertyName("gm_percent")]
    public decimal? GrossMarginPercent { get; set; }

    [JsonPropertyName("crv_value_sales")]
    public decimal? CrvValueSales { get; set; }

    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("row_type")]
    public string? RowType { get; set; }

    [JsonPropertyName("product_class")]
    public string? ProductClass { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("taxable_sales")]
    public decimal TaxableSales { get; set; }

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

    [JsonPropertyName("crv_value_tax")]
    public decimal? CrvValueTax { get; set; }

    [JsonPropertyName("msrp")]
    public string? Msrp { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
