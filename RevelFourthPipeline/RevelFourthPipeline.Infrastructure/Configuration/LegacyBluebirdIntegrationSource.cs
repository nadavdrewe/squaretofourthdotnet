using System.Data;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Infrastructure.Abstractions;

namespace RevelFourthPipeline.Infrastructure.Configuration;

public sealed class LegacyBluebirdIntegrationSource(
    IOptions<RevelFourthPipelineOptions> options,
    IConfiguration configuration,
    ILogger<LegacyBluebirdIntegrationSource> logger)
    : IRevelFourthIntegrationSource
{
    private const string DefaultLegacyWebConfigPath = "BluebirdFourth/web.fourth.revel.com/Web.config";

    private readonly RevelFourthPipelineOptions _options = options.Value;

    public async Task<IReadOnlyList<RevelFourthIntegration>> GetActiveIntegrationsAsync(CancellationToken cancellationToken)
    {
        var connectionString = ResolveConnectionString();
        var integrations = new List<RevelFourthIntegration>();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT
                b.brand_id,
                b.name AS brand_name,
                b.revel_base_url,
                b.key_secret,
                b.fourth_username,
                b.fourth_password,
                b.fourth_locationID AS brand_fourth_locationID,
                b.fourth_RevenueCenter,
                e.DBKEY_establishment_id,
                e.establishment_id,
                e.name AS establishment_name,
                e.fourth_locationID AS establishment_fourth_locationID
            FROM dbo.Brands b
            INNER JOIN dbo.Establishments e ON e.db_brand_id = b.brand_id
            WHERE b.is_fourth_active = 1
              AND e.is_fourth_active = 1
            ORDER BY b.brand_id, e.DBKEY_establishment_id;
            """;
        command.CommandType = CommandType.Text;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var brandId = reader.GetInt32(reader.GetOrdinal("brand_id"));
            var brandName = ReadString(reader, "brand_name");
            var storeName = ReadString(reader, "establishment_name");
            var brandFourthLocation = ReadString(reader, "brand_fourth_locationID");
            var establishmentFourthLocation = ReadString(reader, "establishment_fourth_locationID");

            integrations.Add(new RevelFourthIntegration
            {
                BrandId = brandId,
                BrandName = brandName,
                DatabaseEstablishmentId = reader.GetInt32(reader.GetOrdinal("DBKEY_establishment_id")),
                StoreName = string.IsNullOrWhiteSpace(storeName) ? brandName : storeName,
                RevelEstablishmentId = reader.GetInt32(reader.GetOrdinal("establishment_id")),
                RevelBaseUrl = ReadString(reader, "revel_base_url"),
                RevelApiKeySecret = ReadString(reader, "key_secret"),
                FourthUsername = ReadString(reader, "fourth_username"),
                FourthPassword = ReadString(reader, "fourth_password"),
                FourthOrganisationId = brandFourthLocation,
                FourthLocation = string.IsNullOrWhiteSpace(establishmentFourthLocation)
                    ? brandFourthLocation
                    : establishmentFourthLocation,
                FourthRevenueCentre = FirstNonEmpty(ReadString(reader, "fourth_RevenueCenter"), _options.Fourth.DefaultRevenueCentre)
            });
        }

        logger.LogInformation(
            "Loaded {IntegrationCount} active Revel/Fourth integrations from the legacy Bluebird database.",
            integrations.Count);

        return integrations;
    }

    private string ResolveConnectionString()
    {
        var legacyOptions = _options.LegacyDatabase;

        if (!string.IsNullOrWhiteSpace(legacyOptions.ConnectionString))
        {
            return TrustServerCertificate(legacyOptions.ConnectionString);
        }

        var configured = configuration.GetConnectionString(legacyOptions.ConnectionStringName);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return TrustServerCertificate(configured);
        }

        var webConfigPath = ResolveLegacyWebConfigPath(legacyOptions.LegacyWebConfigPath);
        if (webConfigPath is not null)
        {
            var fromWebConfig = ReadConnectionStringFromWebConfig(webConfigPath, legacyOptions.ConnectionStringName);
            if (!string.IsNullOrWhiteSpace(fromWebConfig))
            {
                return TrustServerCertificate(fromWebConfig);
            }
        }

        throw new InvalidOperationException(
            $"Legacy database is enabled but connection string '{legacyOptions.ConnectionStringName}' could not be resolved.");
    }

    public static string TrustServerCertificate(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            TrustServerCertificate = true
        };

        return builder.ConnectionString;
    }

    private static string? ResolveLegacyWebConfigPath(string configuredPath)
    {
        var candidates = new List<string>();

        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            candidates.Add(configuredPath);
        }

        candidates.Add(DefaultLegacyWebConfigPath);

        foreach (var candidate in candidates)
        {
            var resolved = ResolvePathAgainstAncestors(candidate, Directory.GetCurrentDirectory())
                           ?? ResolvePathAgainstAncestors(candidate, AppContext.BaseDirectory);
            if (resolved is not null)
            {
                return resolved;
            }
        }

        return null;
    }

    private static string? ResolvePathAgainstAncestors(string path, string startDirectory)
    {
        if (Path.IsPathRooted(path))
        {
            return File.Exists(path) ? path : null;
        }

        var directory = new DirectoryInfo(startDirectory);
        while (directory is not null)
        {
            var candidate = Path.GetFullPath(Path.Combine(directory.FullName, path));
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static string? ReadConnectionStringFromWebConfig(string path, string name)
    {
        var document = XDocument.Load(path);
        return document
            .Descendants("connectionStrings")
            .Elements("add")
            .FirstOrDefault(element => string.Equals((string?)element.Attribute("name"), name, StringComparison.OrdinalIgnoreCase))
            ?.Attribute("connectionString")
            ?.Value;
    }

    private static string ReadString(SqlDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? "" : reader.GetString(ordinal);
    }

    private static string FirstNonEmpty(string? preferred, string fallback)
    {
        return string.IsNullOrWhiteSpace(preferred) ? fallback : preferred;
    }
}
