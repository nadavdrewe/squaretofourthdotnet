namespace RevelFourthPipeline.Domain.Fourth;

public sealed class FourthSalesSubmission
{
    public string OrganisationId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public DateTime SalesDate { get; set; }
    public string Location { get; set; } = "";
    public string RevenueCentre { get; set; } = "1";
    public List<FourthSalesTransactionDraft> Transactions { get; set; } = [];
}

public sealed class FourthSalesTransactionDraft
{
    public string Plu { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal Vat { get; set; }
    public decimal TotalNetSales { get; set; }
    public decimal TotalGrossSales { get; set; }
    public string? CategoryCode { get; set; }
    public string SaleType { get; set; } = "1";
}

public sealed class FourthSalesBuildResult
{
    public FourthHeader Header { get; init; } = new();
    public string Xml { get; init; } = "";
}

public sealed class FourthSubmitResult
{
    public double ResultCode { get; init; }
    public string RawResponse { get; init; } = "";
    public bool Succeeded => ResultCode != 0;
}

public sealed class FourthAuthenticationToken
{
    public string SessionId { get; init; } = "";
    public string RawResponse { get; init; } = "";
}
