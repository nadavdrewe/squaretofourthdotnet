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

        var result = new RevelProductMixToFourthMapper().Map(report, new StoreRunContext());

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
}
