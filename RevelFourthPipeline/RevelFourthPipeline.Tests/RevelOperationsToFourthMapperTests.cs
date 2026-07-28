using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;
using RevelFourthPipeline.Infrastructure.Mapping;

namespace RevelFourthPipeline.Tests;

public class RevelOperationsToFourthMapperTests
{
    [Fact]
    public void Map_UsesProductMixRowsAndGroupsByPlu()
    {
        var report = new OperationsReport
        {
            ProductMixData =
            [
                new ProductMixData
                {
                    ProductSku = "1001",
                    ProductName = "Flat White",
                    NumberOfItems = "2",
                    TaxableSales = 10m,
                    UntaxableSales = 0m,
                    Tax = 2m,
                    ProductCategory = "Coffee"
                },
                new ProductMixData
                {
                    ProductSku = "1001",
                    ProductName = "Flat White",
                    NumberOfItems = "1",
                    TaxableSales = 5m,
                    UntaxableSales = 0m,
                    Tax = 1m,
                    ProductCategory = "Coffee"
                },
                new ProductMixData
                {
                    ProductSku = "1001",
                    ProductName = "Total",
                    NumberOfItems = "3",
                    TaxableSales = 15m,
                    Tax = 3m,
                    ProductCategory = "Coffee",
                    RowType = "totals_row"
                }
            ]
        };

        var result = new RevelOperationsToFourthMapper().Map(report, new StoreRunContext());

        var transaction = Assert.Single(result);
        Assert.Equal("1001", transaction.Plu);
        Assert.Equal("Flat White", transaction.Description);
        Assert.Equal(3m, transaction.Quantity);
        Assert.Equal(15m, transaction.TotalNetSales);
        Assert.Equal(3m, transaction.Vat);
        Assert.Equal(18m, transaction.TotalGrossSales);
    }

    [Fact]
    public void Map_FallsBackToPriceMinusTaxWhenSalesBucketsAreEmpty()
    {
        var report = new OperationsReport
        {
            ProductMixData =
            [
                new ProductMixData
                {
                    ProductSku = "2001",
                    ProductName = "Test Item",
                    NumberOfItems = "1",
                    Price = 12m,
                    Tax = 2m
                }
            ]
        };

        var result = new RevelOperationsToFourthMapper().Map(report, new StoreRunContext());

        var transaction = Assert.Single(result);
        Assert.Equal(10m, transaction.TotalNetSales);
        Assert.Equal(12m, transaction.TotalGrossSales);
    }

    [Fact]
    public void Map_RoundsFourthCurrencyFieldsToTwoDecimals()
    {
        var report = new OperationsReport
        {
            ProductMixData =
            [
                new ProductMixData
                {
                    ProductSku = "3001",
                    ProductName = "Precision Item",
                    NumberOfItems = "1",
                    TaxableSales = 10.115m,
                    Tax = 2.225m
                }
            ]
        };

        var result = new RevelOperationsToFourthMapper().Map(report, new StoreRunContext());

        var transaction = Assert.Single(result);
        Assert.Equal(10.12m, transaction.TotalNetSales);
        Assert.Equal(2.23m, transaction.Vat);
        Assert.Equal(12.34m, transaction.TotalGrossSales);
    }
}
