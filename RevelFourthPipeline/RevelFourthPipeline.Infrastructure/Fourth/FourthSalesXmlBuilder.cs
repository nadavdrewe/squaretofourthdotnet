using System.Text;
using System.Xml;
using System.Xml.Serialization;
using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Infrastructure.Abstractions;

namespace RevelFourthPipeline.Infrastructure.Fourth;

public sealed class FourthSalesXmlBuilder : IFourthSalesXmlBuilder
{
    public FourthHeader BuildHeader(FourthSalesSubmission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        var header = new FourthHeader
        {
            OrganisationHeader =
            [
                new FourthOrganisationHeader
                {
                    OrganisationID = submission.OrganisationId,
                    UserName = submission.UserName,
                    Password = submission.Password,
                    SalesHeader =
                    [
                        new FourthSalesHeader
                        {
                            SalesDate = submission.SalesDate.Date,
                            Location = submission.Location,
                            RevenueCentre = submission.RevenueCentre,
                            ActionIfDataExists = FourthActionIfDataExists.ReplaceExisting,
                            SalesTransaction = submission.Transactions
                                .Select(ToFourthSalesTransaction)
                                .ToList()
                        }
                    ]
                }
            ]
        };

        return header;
    }

    public XmlDocument BuildXmlDocument(FourthSalesSubmission submission)
    {
        var header = BuildHeader(submission);
        var document = SerializeToDocument(header);

        if (document.DocumentElement is not null)
        {
            document.DocumentElement.SetAttribute("xmlns", "");
            StripNamespaces(document.DocumentElement);
        }

        return document;
    }

    public FourthSalesBuildResult BuildXml(FourthSalesSubmission submission)
    {
        var header = BuildHeader(submission);
        var document = SerializeToDocument(header);

        if (document.DocumentElement is not null)
        {
            document.DocumentElement.SetAttribute("xmlns", "");
            StripNamespaces(document.DocumentElement);
        }

        return new FourthSalesBuildResult
        {
            Header = header,
            Xml = document.OuterXml
        };
    }

    private static FourthSalesTransaction ToFourthSalesTransaction(FourthSalesTransactionDraft draft)
    {
        var netUnit = draft.Quantity == 0 ? 0 : draft.TotalNetSales / draft.Quantity;
        var grossUnit = draft.Quantity == 0 ? 0 : draft.TotalGrossSales / draft.Quantity;

        return new FourthSalesTransaction
        {
            PLU = draft.Plu,
            Description = draft.Description,
            Quantity = draft.Quantity,
            VAT = RoundCurrency(draft.Vat),
            TotalGrossSales = RoundCurrency(draft.TotalGrossSales),
            NetSalesPrice = RoundCurrency(netUnit),
            NetSalesPriceSpecified = true,
            GrossSalesPrice = RoundCurrency(grossUnit),
            GrossSalesPriceSpecified = true,
            TotalNetSales = RoundCurrency(draft.TotalNetSales),
            TotalNetSalesSpecified = true,
            CategoryCode = draft.CategoryCode,
            SaleType = draft.SaleType
        };
    }

    private static XmlDocument SerializeToDocument(FourthHeader header)
    {
        var serializer = new XmlSerializer(typeof(FourthHeader));
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", "");

        using var stream = new MemoryStream();
        var writerSettings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            CloseOutput = false,
            OmitXmlDeclaration = false
        };

        using (var writer = XmlWriter.Create(stream, writerSettings))
        {
            serializer.Serialize(writer, header, namespaces);
        }

        stream.Position = 0;
        var document = new XmlDocument
        {
            PreserveWhitespace = false
        };
        document.Load(stream);
        return document;
    }

    private static void StripNamespaces(XmlNode node)
    {
        if (node.NodeType == XmlNodeType.Element)
        {
            var element = (XmlElement)node;
            if (!string.IsNullOrEmpty(element.NamespaceURI))
            {
                var newElement = element.OwnerDocument.CreateElement(element.LocalName);

                while (element.HasAttributes)
                {
                    var attribute = element.Attributes[0]!;
                    element.RemoveAttributeNode(attribute);
                    if (!attribute.Name.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase))
                    {
                        newElement.Attributes.Append(attribute);
                    }
                }

                while (element.HasChildNodes)
                {
                    newElement.AppendChild(element.FirstChild!);
                }

                element.ParentNode?.ReplaceChild(newElement, element);
                element = newElement;
            }
        }

        foreach (var child in node.ChildNodes.Cast<XmlNode>().ToList())
        {
            StripNamespaces(child);
        }
    }

    private static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
