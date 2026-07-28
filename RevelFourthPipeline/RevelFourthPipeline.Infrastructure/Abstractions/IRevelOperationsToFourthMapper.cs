using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IRevelOperationsToFourthMapper
{
    IReadOnlyList<FourthSalesTransactionDraft> Map(OperationsReport report, StoreRunContext context);
}
