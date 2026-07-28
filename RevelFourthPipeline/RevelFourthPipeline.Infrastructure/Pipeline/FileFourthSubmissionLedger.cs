using System.Text.Json;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Infrastructure.Abstractions;

namespace RevelFourthPipeline.Infrastructure.Pipeline;

public sealed class FileFourthSubmissionLedger(
    IOptions<RevelFourthPipelineOptions> options)
    : IFourthSubmissionLedger
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly RunLedgerOptions _options = options.Value.RunLedger;

    public async Task<bool> HasSuccessfulSubmissionAsync(
        FourthSubmissionLedgerEntry entry,
        CancellationToken cancellationToken)
    {
        if (!_options.Enabled || !File.Exists(ResolvePath()))
        {
            return false;
        }

        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await foreach (var existing in ReadEntriesAsync(cancellationToken).ConfigureAwait(false))
            {
                if (existing.IsSameSubmission(entry))
                {
                    return true;
                }
            }

            return false;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RecordSuccessfulSubmissionAsync(
        FourthSubmissionLedgerEntry entry,
        CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var path = ResolvePath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var line = JsonSerializer.Serialize(entry, JsonOptions);
            await File.AppendAllTextAsync(path, line + Environment.NewLine, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async IAsyncEnumerable<FourthSubmissionLedgerEntry> ReadEntriesAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var path = ResolvePath();
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            FourthSubmissionLedgerEntry? entry;
            try
            {
                entry = JsonSerializer.Deserialize<FourthSubmissionLedgerEntry>(line, JsonOptions);
            }
            catch (JsonException)
            {
                continue;
            }

            if (entry is not null)
            {
                yield return entry;
            }
        }
    }

    private string ResolvePath()
    {
        return Path.IsPathRooted(_options.Path)
            ? _options.Path
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _options.Path));
    }
}
