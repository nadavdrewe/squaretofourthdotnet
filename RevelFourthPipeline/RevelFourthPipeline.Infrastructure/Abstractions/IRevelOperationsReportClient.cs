using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IRevelOperationsReportClient
{
    Uri BuildOperationsReportUri(RevelOperationsRequest request);
    Task<OperationsReport> GetOperationsReportAsync(RevelOperationsRequest request, CancellationToken cancellationToken);
}
