using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Infrastructure.Abstractions;

namespace RevelFourthPipeline.Infrastructure.Configuration;

public sealed class OptionsRevelFourthIntegrationSource(
    IOptions<RevelFourthPipelineOptions> options)
    : IRevelFourthIntegrationSource
{
    private readonly RevelFourthPipelineOptions _options = options.Value;

    public Task<IReadOnlyList<RevelFourthIntegration>> GetActiveIntegrationsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<RevelFourthIntegration> integrations = _options.Stores
            .Where(store => store.Active)
            .Select(store => new RevelFourthIntegration
            {
                BrandId = store.BrandId,
                BrandName = string.IsNullOrWhiteSpace(store.BrandName) ? store.Name : store.BrandName,
                DatabaseEstablishmentId = store.DatabaseEstablishmentId,
                StoreName = store.Name,
                RevelEstablishmentId = store.RevelEstablishmentId,
                RevelBaseUrl = FirstNonEmpty(store.RevelBaseUrl, _options.Revel.BaseUrl),
                RevelApiKeySecret = FirstNonEmpty(store.RevelApiKeySecret, _options.Revel.ApiKeySecret),
                FourthUsername = FirstNonEmpty(store.FourthUsername, _options.Fourth.Username),
                FourthPassword = FirstNonEmpty(store.FourthPassword, _options.Fourth.Password),
                FourthOrganisationId = FirstNonEmpty(store.FourthOrganisationId, _options.Fourth.OrganisationId),
                FourthLocation = FirstNonEmpty(store.FourthLocation, _options.Fourth.DefaultLocation),
                FourthRevenueCentre = FirstNonEmpty(store.FourthRevenueCentre, _options.Fourth.DefaultRevenueCentre)
            })
            .ToList();

        return Task.FromResult(integrations);
    }

    private static string FirstNonEmpty(string? preferred, string fallback)
    {
        return string.IsNullOrWhiteSpace(preferred) ? fallback : preferred;
    }
}
