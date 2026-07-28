using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Infrastructure.Abstractions;

namespace RevelFourthPipeline.Worker;

public class Worker(
    IRevelFourthPipelineRunner runner,
    IOptions<RevelFourthPipelineOptions> options,
    IHostApplicationLifetime applicationLifetime,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pipelineOptions = options.Value;

        if (!pipelineOptions.RunOnStartup)
        {
            logger.LogInformation("RunOnStartup is false; waiting until next scheduled run.");
            await DelayUntilNextRunAsync(pipelineOptions.BusinessDayStartHour, stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var ranges = BusinessDayRangeResolver.Resolve(pipelineOptions, DateTime.Now);

            for (var rangeIndex = 0; rangeIndex < ranges.Count; rangeIndex++)
            {
                var range = ranges[rangeIndex];

                try
                {
                    logger.LogInformation(
                        "Starting Revel Fourth pipeline range {RangeIndex}/{RangeCount} for {RangeStart} to {RangeEnd}. DryRun={DryRun}",
                        rangeIndex + 1,
                        ranges.Count,
                        range.RangeStart,
                        range.RangeEnd,
                        pipelineOptions.DryRun);

                    var results = await runner.RunForRangeAsync(range.RangeStart, range.RangeEnd, stoppingToken);
                    var succeeded = results.Count(x => x.Succeeded);
                    var failed = results.Count - succeeded;

                    logger.LogInformation(
                        "Revel Fourth pipeline range {RangeIndex}/{RangeCount} completed. Stores={StoreCount}, Succeeded={Succeeded}, Failed={Failed}",
                        rangeIndex + 1,
                        ranges.Count,
                        results.Count,
                        succeeded,
                        failed);

                    foreach (var result in results)
                    {
                        logger.LogInformation(
                            "Store {StoreName}: Success={Succeeded}, DryRun={DryRun}, FourthLoginValidated={FourthLoginValidated}, Transactions={TransactionCount}, Message={Message}",
                            result.Context.StoreName,
                            result.Succeeded,
                            result.DryRun,
                            result.FourthLoginValidated,
                            result.Transactions.Count,
                            result.Message);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Revel Fourth pipeline run failed for {RangeStart} to {RangeEnd}; worker will continue with remaining ranges or wait until the next scheduled run.",
                        range.RangeStart,
                        range.RangeEnd);
                }
            }

            if (pipelineOptions.RunOnce)
            {
                logger.LogInformation("RunOnce is true; stopping worker.");
                applicationLifetime.StopApplication();
                return;
            }

            await DelayUntilNextRunAsync(pipelineOptions.BusinessDayStartHour, stoppingToken);
        }
    }

    private static Task DelayUntilNextRunAsync(int businessDayStartHour, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var todayStart = new DateTime(now.Year, now.Month, now.Day, businessDayStartHour, 0, 0);
        var nextRun = now < todayStart ? todayStart : todayStart.AddDays(1);
        var delay = nextRun - now;
        return Task.Delay(delay, cancellationToken);
    }
}
