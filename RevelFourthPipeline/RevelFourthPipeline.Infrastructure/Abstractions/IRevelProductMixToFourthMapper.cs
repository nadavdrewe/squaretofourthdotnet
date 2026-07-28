using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IRevelProductMixToFourthMapper
{
    IReadOnlyList<FourthSalesTransactionDraft> Map(ProductMixReport report, StoreRunContext context);
}
