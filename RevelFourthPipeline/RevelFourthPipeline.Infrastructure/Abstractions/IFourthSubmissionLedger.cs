using RevelFourthPipeline.Domain.Pipeline;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IFourthSubmissionLedger
{
    Task<bool> HasSuccessfulSubmissionAsync(
        FourthSubmissionLedgerEntry entry,
        CancellationToken cancellationToken);

    Task RecordSuccessfulSubmissionAsync(
        FourthSubmissionLedgerEntry entry,
        CancellationToken cancellationToken);
}

public sealed class FourthSubmissionLedgerEntry
{
    public string Source { get; init; } = "";
    public int RevelEstablishmentId { get; init; }
    public string FourthLocation { get; init; } = "";
    public string FourthRevenueCentre { get; init; } = "";
    public DateTime BusinessDate { get; init; }
    public DateTime RangeStart { get; init; }
    public DateTime RangeEnd { get; init; }
    public string PayloadSha256 { get; init; } = "";
    public DateTimeOffset SubmittedAtUtc { get; init; }

    public static FourthSubmissionLedgerEntry Create(
        string source,
        StoreRunContext context,
        string payloadSha256)
    {
        return new FourthSubmissionLedgerEntry
        {
            Source = source,
            RevelEstablishmentId = context.RevelEstablishmentId,
            FourthLocation = context.FourthLocation,
            FourthRevenueCentre = context.FourthRevenueCentre,
            BusinessDate = context.RangeStart.Date,
            RangeStart = context.RangeStart,
            RangeEnd = context.RangeEnd,
            PayloadSha256 = payloadSha256,
            SubmittedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public bool IsSameSubmission(FourthSubmissionLedgerEntry other)
    {
        return string.Equals(Source, other.Source, StringComparison.OrdinalIgnoreCase)
               && RevelEstablishmentId == other.RevelEstablishmentId
               && string.Equals(FourthLocation, other.FourthLocation, StringComparison.OrdinalIgnoreCase)
               && string.Equals(FourthRevenueCentre, other.FourthRevenueCentre, StringComparison.OrdinalIgnoreCase)
               && BusinessDate == other.BusinessDate
               && RangeStart == other.RangeStart
               && RangeEnd == other.RangeEnd
               && string.Equals(PayloadSha256, other.PayloadSha256, StringComparison.OrdinalIgnoreCase);
    }
}
