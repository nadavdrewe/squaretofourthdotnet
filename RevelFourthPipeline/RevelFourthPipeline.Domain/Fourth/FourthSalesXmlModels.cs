using System.ComponentModel;
using System.Xml.Serialization;

namespace RevelFourthPipeline.Domain.Fourth;

[Serializable]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false)]
public sealed class FourthHeader
{
    [XmlElement("OrganisationHeader")]
    public List<FourthOrganisationHeader> OrganisationHeader { get; set; } = [];
}

[Serializable]
[XmlType(AnonymousType = true)]
public sealed class FourthOrganisationHeader
{
    public string OrganisationID { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";

    [XmlElement("SalesHeader")]
    public List<FourthSalesHeader> SalesHeader { get; set; } = [];
}

[Serializable]
[XmlType(AnonymousType = true)]
public sealed class FourthSalesHeader
{
    public FourthSalesHeader()
    {
        Location = "1";
        RevenueCentre = "1";
        ActionIfDataExists = FourthActionIfDataExists.ReplaceExisting;
    }

    [XmlElement(DataType = "date")]
    public DateTime SalesDate { get; set; }

    public string Location { get; set; }
    public string RevenueCentre { get; set; }

    [DefaultValue(FourthActionIfDataExists.ReplaceExisting)]
    public FourthActionIfDataExists ActionIfDataExists { get; set; }

    [XmlElement("SalesTransaction")]
    public List<FourthSalesTransaction> SalesTransaction { get; set; } = [];
}

[Serializable]
public enum FourthActionIfDataExists
{
    [XmlEnum("1")]
    Append = 1,

    [XmlEnum("2")]
    ReplaceExisting = 2,

    [XmlEnum("3")]
    Ignore = 3
}

[Serializable]
[XmlType(AnonymousType = true)]
public sealed class FourthSalesTransaction
{
    public FourthSalesTransaction()
    {
        SaleType = "1";
    }

    public string PLU { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal VAT { get; set; }
    public decimal TotalGrossSales { get; set; }
    public decimal NetSalesPrice { get; set; }

    [XmlIgnore]
    public bool NetSalesPriceSpecified { get; set; }

    public decimal GrossSalesPrice { get; set; }

    [XmlIgnore]
    public bool GrossSalesPriceSpecified { get; set; }

    public decimal TotalNetSales { get; set; }

    [XmlIgnore]
    public bool TotalNetSalesSpecified { get; set; }

    public string? CategoryCode { get; set; }
    public string SaleType { get; set; }
}
