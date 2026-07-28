using RevelFourthPipeline.Domain.Fourth;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IFourthSoapClient
{
    Task<FourthAuthenticationToken> LoginAsync(string userName, string password, CancellationToken cancellationToken);
    Task<FourthSubmitResult> SubmitSalesAsync(FourthAuthenticationToken token, string salesXml, CancellationToken cancellationToken);
}
