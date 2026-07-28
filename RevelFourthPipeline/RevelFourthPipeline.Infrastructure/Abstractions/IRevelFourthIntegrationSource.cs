using RevelFourthPipeline.Domain.Pipeline;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IRevelFourthIntegrationSource
{
    Task<IReadOnlyList<RevelFourthIntegration>> GetActiveIntegrationsAsync(CancellationToken cancellationToken);
}
