using System.Globalization;
using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;
using RevelFourthPipeline.Infrastructure.Abstractions;

namespace RevelFourthPipeline.Infrastructure.Mapping;

public sealed class RevelOperationsToFourthMapper : IRevelOperationsToFourthMapper
{
    public IReadOnlyList<FourthSalesTransactionDraft> Map(OperationsReport report, StoreRunContext context)
    {
        if (report.ProductMixData.Count == 0)
        {
            return [];
        }

        return report.ProductMixData
            .Where(IsProductRow)
            .Select(row => MapProductRow(row))
            .Where(row => row.Quantity != 0 || row.TotalNetSales != 0 || row.Vat != 0 || row.TotalGrossSales != 0)
            .GroupBy(row => row.Plu, StringComparer.OrdinalIgnoreCase)
            .Select(group => Merge(group))
            .OrderBy(row => row.Description, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool IsProductRow(ProductMixData row)
    {
        if (string.Equals(row.RowType, "totals_row", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(row.ProductSku)
               || !string.IsNullOrWhiteSpace(row.ProductName)
               || !string.IsNullOrWhiteSpace(row.ProductDescription);
    }

    private static FourthSalesTransactionDraft MapProductRow(ProductMixData row)
    {
        var quantity = ParseDecimal(row.NumberOfItems);
        var netSales = row.TaxableSales + row.UntaxableSales;

        if (netSales == 0 && row.Price != 0)
        {
            netSales = row.Price - row.Tax;
        }

        var vat = row.Tax;
        var grossSales = netSales + vat;

        return new FourthSalesTransactionDraft
        {
            Plu = BuildPlu(row),
            Description = BuildDescription(row),
            Quantity = quantity,
            Vat = vat,
            TotalNetSales = netSales,
            TotalGrossSales = grossSales,
            CategoryCode = row.ProductCategory
        };
    }

    private static FourthSalesTransactionDraft Merge(IEnumerable<FourthSalesTransactionDraft> rows)
    {
        var materialized = rows.ToList();
        var first = materialized.First();

        return new FourthSalesTransactionDraft
        {
            Plu = first.Plu,
            Description = first.Description,
            Quantity = materialized.Sum(x => x.Quantity),
            Vat = RoundCurrency(materialized.Sum(x => x.Vat)),
            TotalNetSales = RoundCurrency(materialized.Sum(x => x.TotalNetSales)),
            TotalGrossSales = RoundCurrency(materialized.Sum(x => x.TotalGrossSales)),
            CategoryCode = first.CategoryCode,
            SaleType = first.SaleType
        };
    }

    private static string BuildPlu(ProductMixData row)
    {
        var sku = Normalize(row.ProductSku);
        if (!string.IsNullOrWhiteSpace(sku) && !sku.Equals("UNIDENTIFIED", StringComparison.OrdinalIgnoreCase))
        {
            return sku;
        }

        var barcode = Normalize(row.ProductBarcode);
        if (!string.IsNullOrWhiteSpace(barcode))
        {
            return barcode;
        }

        var description = BuildDescription(row);
        return description.Length <= 32 ? description : description[..32];
    }

    private static string BuildDescription(ProductMixData row)
    {
        return Normalize(row.ProductName)
               ?? Normalize(row.ProductDescription)
               ?? Normalize(row.ProductSku)
               ?? "Unidentified Item";
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0m;
        }

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var invariant))
        {
            return invariant;
        }

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var current))
        {
            return current;
        }

        return 0m;
    }

    private static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
