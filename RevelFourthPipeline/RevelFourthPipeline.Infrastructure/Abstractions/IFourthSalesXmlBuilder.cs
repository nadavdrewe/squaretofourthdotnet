using System.Xml;
using RevelFourthPipeline.Domain.Fourth;

namespace RevelFourthPipeline.Infrastructure.Abstractions;

public interface IFourthSalesXmlBuilder
{
    FourthHeader BuildHeader(FourthSalesSubmission submission);
    XmlDocument BuildXmlDocument(FourthSalesSubmission submission);
    FourthSalesBuildResult BuildXml(FourthSalesSubmission submission);
}
