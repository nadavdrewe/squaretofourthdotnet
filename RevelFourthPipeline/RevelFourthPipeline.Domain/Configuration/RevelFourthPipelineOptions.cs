namespace RevelFourthPipeline.Domain.Configuration;

public sealed class RevelFourthPipelineOptions
{
    public const string SectionName = "RevelFourthPipeline";

    public bool DryRun { get; set; } = true;
    public bool ValidateFourthLoginInDryRun { get; set; }
    public bool RunOnStartup { get; set; } = true;
    public bool RunOnce { get; set; } = true;
    public bool SplitOverrideRangeIntoDailyRuns { get; set; } = true;
    public int BusinessDayStartHour { get; set; } = 4;
    public DateTime? OverrideRangeStart { get; set; }
    public DateTime? OverrideRangeEnd { get; set; }
    public LegacyDatabaseOptions LegacyDatabase { get; set; } = new();
    public RevelOptions Revel { get; set; } = new();
    public FourthOptions Fourth { get; set; } = new();
    public RunLedgerOptions RunLedger { get; set; } = new();
    public List<StorePipelineOptions> Stores { get; set; } = [];
}

public sealed class LegacyDatabaseOptions
{
    public bool Enabled { get; set; }
    public string ConnectionStringName { get; set; } = "RevelContext";
    public string ConnectionString { get; set; } = "";
    public string LegacyWebConfigPath { get; set; } = "";
}

public sealed class RevelOptions
{
    public string BaseUrl { get; set; } = "";
    public string ApiKeySecret { get; set; } = "";
    public int TimeoutSeconds { get; set; } = 600;
    public string SalesReportSource { get; set; } = "ProductMix";
}

public sealed class FourthOptions
{
    public string SoapEndpoint { get; set; } = "http://ws.fourthhospitality.com/fhapi.asmx";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string OrganisationId { get; set; } = "";
    public string DefaultLocation { get; set; } = "";
    public string DefaultRevenueCentre { get; set; } = "1";
}

public sealed class RunLedgerOptions
{
    public bool Enabled { get; set; } = true;
    public bool AllowDuplicateLiveSubmissions { get; set; }
    public string Path { get; set; } = "data/revel-fourth-live-submissions.jsonl";
}

public sealed class StorePipelineOptions
{
    public int? BrandId { get; set; }
    public string BrandName { get; set; } = "";
    public string Name { get; set; } = "";
    public int? DatabaseEstablishmentId { get; set; }
    public int RevelEstablishmentId { get; set; }
    public string? RevelBaseUrl { get; set; }
    public string? RevelApiKeySecret { get; set; }
    public string? FourthUsername { get; set; }
    public string? FourthPassword { get; set; }
    public string? FourthOrganisationId { get; set; }
    public string? FourthLocation { get; set; }
    public string? FourthRevenueCentre { get; set; }
    public bool Active { get; set; } = true;
}
