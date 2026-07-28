using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Domain.Revel;

namespace RevelFourthPipeline.Domain.Pipeline;

public sealed class RevelOperationsRequest
{
    public string StoreName { get; init; } = "";
    public int RevelEstablishmentId { get; init; }
    public string? RevelBaseUrl { get; init; }
    public string? RevelApiKeySecret { get; init; }
    public DateTime RangeStart { get; init; }
    public DateTime RangeEnd { get; init; }
}

public sealed class StoreRunContext
{
    public int? BrandId { get; init; }
    public string BrandName { get; init; } = "";
    public int? DatabaseEstablishmentId { get; init; }
    public string StoreName { get; init; } = "";
    public int RevelEstablishmentId { get; init; }
    public string FourthLocation { get; init; } = "";
    public string FourthRevenueCentre { get; init; } = "1";
    public DateTime RangeStart { get; init; }
    public DateTime RangeEnd { get; init; }
}

public sealed class StorePipelineRunResult
{
    public StoreRunContext Context { get; init; } = new();
    public object? SourceReport { get; init; }
    public List<FourthSalesTransactionDraft> Transactions { get; init; } = [];
    public string FourthXml { get; init; } = "";
    public FourthSubmitResult? SubmitResult { get; init; }
    public bool DryRun { get; init; }
    public bool FourthLoginValidated { get; init; }
    public bool Succeeded { get; init; }
    public string Message { get; init; } = "";
}

public sealed class RevelProductMixRequest
{
    public string StoreName { get; init; } = "";
    public int RevelEstablishmentId { get; init; }
    public string? RevelBaseUrl { get; init; }
    public string? RevelApiKeySecret { get; init; }
    public DateTime RangeStart { get; init; }
    public DateTime RangeEnd { get; init; }
}

public sealed class RevelFourthIntegration
{
    public int? BrandId { get; init; }
    public string BrandName { get; init; } = "";
    public int? DatabaseEstablishmentId { get; init; }
    public string StoreName { get; init; } = "";
    public int RevelEstablishmentId { get; init; }
    public string RevelBaseUrl { get; init; } = "";
    public string RevelApiKeySecret { get; init; } = "";
    public string FourthUsername { get; init; } = "";
    public string FourthPassword { get; init; } = "";
    public string FourthOrganisationId { get; init; } = "";
    public string FourthLocation { get; init; } = "";
    public string FourthRevenueCentre { get; init; } = "1";
}
