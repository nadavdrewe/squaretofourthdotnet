using System.Xml.Linq;
using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Infrastructure.Fourth;

namespace RevelFourthPipeline.Tests;

public class FourthSalesXmlBuilderTests
{
    [Fact]
    public void BuildXml_ProducesFourthHeaderSalesTransactionShape()
    {
        var result = new FourthSalesXmlBuilder().BuildXml(new FourthSalesSubmission
        {
            OrganisationId = "ORG1",
            UserName = "user",
            Password = "pass",
            SalesDate = new DateTime(2026, 6, 6),
            Location = "LOC1",
            RevenueCentre = "1",
            Transactions =
            [
                new FourthSalesTransactionDraft
                {
                    Plu = "1001",
                    Description = "Flat White",
                    Quantity = 2m,
                    Vat = 2m,
                    TotalNetSales = 10m,
                    TotalGrossSales = 12m,
                    CategoryCode = "Coffee"
                }
            ]
        });

        var document = XDocument.Parse(result.Xml);
        var root = Assert.IsType<XElement>(document.Root);

        Assert.Equal("FourthHeader", root.Name.LocalName);
        Assert.Empty(root.Name.NamespaceName);
        Assert.Equal("ORG1", root.Descendants("OrganisationID").Single().Value);
        Assert.Equal("LOC1", root.Descendants("Location").Single().Value);
        Assert.Equal("1001", root.Descendants("PLU").Single().Value);
        Assert.Equal("Flat White", root.Descendants("Description").Single().Value);
        Assert.Equal("2", root.Descendants("Quantity").Single().Value);
        Assert.Equal("10", root.Descendants("TotalNetSales").Single().Value);
        Assert.Equal("12", root.Descendants("TotalGrossSales").Single().Value);
        Assert.Equal("5", root.Descendants("NetSalesPrice").Single().Value);
        Assert.Equal("6", root.Descendants("GrossSalesPrice").Single().Value);
    }
}
