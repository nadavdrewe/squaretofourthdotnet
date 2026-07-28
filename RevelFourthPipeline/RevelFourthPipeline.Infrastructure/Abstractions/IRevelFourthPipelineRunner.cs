using RevelFourthPipeline.Domain.Pipeline;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IRevelFourthPipelineRunner
{
    Task<IReadOnlyList<StorePipelineRunResult>> RunForRangeAsync(
        DateTime rangeStart,
        DateTime rangeEnd,
        CancellationToken cancellationToken);
}
