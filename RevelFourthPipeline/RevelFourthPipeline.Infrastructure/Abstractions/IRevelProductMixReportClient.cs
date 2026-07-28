using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IRevelProductMixReportClient
{
    Uri BuildProductMixReportUri(RevelProductMixRequest request);
    Task<ProductMixReport> GetProductMixReportAsync(RevelProductMixRequest request, CancellationToken cancellationToken);
}
