using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;
using RevelFourthPipeline.Infrastructure.Mapping;

namespace RevelFourthPipeline.Tests;

public class RevelProductMixToFourthMapperTests
{
    [Fact]
    public void Map_UsesOnlyProductRowsAndGroupsBySku()
    {
        var report = new ProductMixReport
        {
            ProductMix =
            [
                new ProductMixRow
                {
                    ProductSku = "1001",
                    ProductName = "Flat White",
                    RowType = "Product",
                    NumberOfItems = "2",
                    TaxableSales = 10m,
                    Tax = 2m,
                    ProductCategory = "Coffee"
                },
                new ProductMixRow
                {
                    ProductSku = "1001",
                    ProductName = "Flat White",
                    RowType = "Product",
                    NumberOfItems = "1",
                    TaxableSales = 5m,
                    Tax = 1m,
                    ProductCategory = "Coffee"
                },
                new ProductMixRow
                {
                    ProductName = "Coffee / tea",
                    RowType = "Class",
                    NumberOfItems = "3",
                    TaxableSales = 15m,
                    Tax = 3m
                },
                new ProductMixRow
                {
                    ProductSku = "2001",
                    ProductName = "Bellini Elderflower",
                    RowType = "Parent_Product",
                    NumberOfItems = "1",
                    TaxableSales = 6m,
                    Tax = 1.2m
                },
                new ProductMixRow
                {
                    ProductName = "Total",
                    RowType = "Totals",
                    NumberOfItems = "3",
                    TaxableSales = 15m,
                    Tax = 3m
                }
            ]
        };

        var result = CreateMapper().Map(report, new StoreRunContext());

        Assert.Equal(2, result.Count);

        var transaction = Assert.Single(result, x => x.Plu == "1001");
        Assert.Equal("1001", transaction.Plu);
        Assert.Equal("Flat White", transaction.Description);
        Assert.Equal(3m, transaction.Quantity);
        Assert.Equal(15m, transaction.TotalNetSales);
        Assert.Equal(3m, transaction.Vat);
        Assert.Equal(18m, transaction.TotalGrossSales);

        var parentProduct = Assert.Single(result, x => x.Plu == "2001");
        Assert.Equal("Bellini Elderflower", parentProduct.Description);
        Assert.Equal(6m, parentProduct.TotalNetSales);
    }

    [Fact]
    public void Map_ExcludesProductAndParentProductNamesContainingConfiguredValues()
    {
        var report = new ProductMixReport
        {
            ProductMix =
            [
                new ProductMixRow
                {
                    ProductSku = "wine-1",
                    ProductName = "Ruinart Blanc de Blancs Bottle",
                    RowType = "Product",
                    NumberOfItems = "3",
                    TaxableSales = 300m,
                    Tax = 60m
                },
                new ProductMixRow
                {
                    ProductSku = "wine-2",
                    ProductName = "Large Bottle",
                    ParentProductName = "Cloudy Bay Sauvignon Blanc",
                    RowType = "Parent_Product",
                    NumberOfItems = "5",
                    TaxableSales = 250m,
                    Tax = 50m
                },
                new ProductMixRow
                {
                    ProductSku = "1001",
                    ProductName = "Flat White",
                    RowType = "Product",
                    NumberOfItems = "2",
                    TaxableSales = 10m,
                    Tax = 2m
                }
            ]
        };

        var mapper = CreateMapper("ruinart blanc de blancs", "CLOUDY BAY SAUVIGNON BLANC");

        var result = mapper.Map(report, new StoreRunContext());

        var transaction = Assert.Single(result);
        Assert.Equal("1001", transaction.Plu);
        Assert.Equal("Flat White", transaction.Description);
    }

    private static RevelProductMixToFourthMapper CreateMapper(params string[] exclusions)
    {
        return new RevelProductMixToFourthMapper(Options.Create(new RevelFourthPipelineOptions
        {
            Revel = new RevelOptions
            {
                ExcludedProductNameContains = exclusions.ToList()
            }
        }));
    }
}
