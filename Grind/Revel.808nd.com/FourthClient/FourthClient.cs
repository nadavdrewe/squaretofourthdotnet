using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using extension.railgunit.com.XML;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.FourthModelMapping;
using Revel._808nd.com.fhAPI;
using Revel._808nd.com.Models;
using System.Threading.Tasks;
using Revel._808nd.com.Helper;

namespace Revel._808nd.com.FourthClient
{
    public class FourthClient
    {
        public FourthClient()
        {
            fhAPI = new fhAPISoapClient();
            Logger = new RevelContext();
        }

        public FourthClient(RevelContextBase logger)
        {
            fhAPI = new fhAPISoapClient();
            Logger = logger;
        }


        private RevelContextBase Logger { get; set; }
        public fhAPISoapClient fhAPI { get; set; }
        public AuthenticationHeader LoginToken { get; set; }
        public bool isLoggedIn { get; set; }

        public AuthenticationHeader Login(string user, string password)
        {
            try
            {
                var token = fhAPI.Login(user, password);

                this.LoginToken = token;
                this.isLoggedIn = true;

            }
            catch (Exception ex)
            {

                throw new Exception("Fourth Client was unable to log in", ex);
            }

            return this.LoginToken;
        }


        public async Task SubmitTimesheetsToFourth(IEnumerable<FourthTimeSheetEntry> timesheets, 
            Brand brand, 
            DateTime timesheetDateTime, 
            Establishment establishment = null)
        {

            FourthHeader fourthHeader = GenerateBaseFourthHeader(brand);
            var objectToSerialsie = new FourthTimeSheetRoot
            { };


            var clockInSheets = new List<FourthTimeSheetEntry>();
            timesheets.ToList().ForEach(sheet => clockInSheets.Add(new FourthTimeSheetEntry
            {
                CheckIn = Convert.ToDateTime(sheet.CheckIn),
                //CheckOut = null,
                ClockStatus = 1,
                EmpNo = sheet.EmpNo,
                Location = sheet.Location,
                Notes = sheet.Notes
            }));

            var clockOutSheets = new List<FourthTimeSheetEntry>();

            timesheets.ToList().ForEach(sheet => clockOutSheets.Add(new FourthTimeSheetEntry
            {// CheckIn = null,
                CheckOut = Convert.ToDateTime(sheet.CheckOut),
                ClockStatus = 0,
                EmpNo = sheet.EmpNo,
                Location = sheet.Location,
                Notes = sheet.Notes
            }));

            var combinedList = clockInSheets.Concat(clockOutSheets).ToList();
            objectToSerialsie.Records = combinedList.ToList(); //timesheets.ToList(); //toList
            var xml = objectToSerialsie.ToXML();
            //xml.DocumentElement.SetAttribute("xmlns", "");
            xml.DocumentElement.SetAttribute("GroupGUID", brand.fourth_guid.ToString());
            xml.DocumentElement.SetAttribute("DateTime", timesheetDateTime.ToString("yyyy-MM-ddThh:ss:mm"));

            foreach (var item in xml.DocumentElement.ChildNodes)
            {
                var element = item as XmlElement;

                foreach (var childItem in element.ChildNodes)
                {
                    var childElement = childItem as XmlElement;
                    childElement.RemoveAllAttributes();
                }

            }
            new XmlStripper().RemoveAllNamespaces(xml.DocumentElement);
            var childNOdes = xml.ChildNodes;


            var xmlString = xml.ConvertXMLDocToString();
            // single call list of records I assume
            try
            {
               // var response = await fhAPI.SubmitTandAAsync(this.LoginToken, xml);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public double SubmitSalesRequestToFourth(
            IEnumerable<RevelSummedOrderItems> summedItems,
            Brand brand,
            out string XML,
            Establishment establishment = null)
        {
            if (summedItems == null) throw new ArgumentNullException(nameof(summedItems));

            // Materialize once
            var items = summedItems.ToList();

            // Same PLU logic as GenerateFourthHeaderForSales (KEEP THESE IN SYNC)
            string BuildPlu(RevelSummedOrderItems x)
            {
                var sku = (x.SKU ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(sku) &&
                    !sku.Equals("UNIDENTIFIED", StringComparison.OrdinalIgnoreCase))
                {
                    return sku;
                }

                if (x.PRODUCT_ID > 0) return x.PRODUCT_ID.ToString();

                return x.DBKEY_ID.ToString();
            }

             string BuildDescription(RevelSummedOrderItems x)
            {
                var name = (x.NAME ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(name)) return name;

                var sku = (x.SKU ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(sku)) return sku;

                if (x.PRODUCT_ID > 0) return $"Product {x.PRODUCT_ID}";

                return "Unidentified Item";
            }

            // ----------------------------
            // Reconciliation: per-product + day totals
            // ----------------------------
            var perProduct = items
                .Where(x => x != null)
                .GroupBy(x => BuildPlu(x))
                .Select(g =>
                {
                    var qty = g.Sum(x => x.QUANTITY);
                    var net = g.Sum(x => x.PURE_SALES);          // Product Mix "Total Sales"
            var vat = g.Sum(x => x.TAX);
                    var gross = g.Sum(x => x.PURE_SALES_PLUS_TAX);

                    var first = g
                        .OrderByDescending(x => !string.IsNullOrWhiteSpace(x.NAME))
                        .ThenByDescending(x => x.QUANTITY)
                        .First();

                    return new
                    {
                        PLU = g.Key,
                        Name = BuildDescription(first),
                        Qty = qty,
                        NetSales = net,
                        Vat = vat,
                        GrossSales = gross
                    };
                })
                .OrderByDescending(x => x.NetSales)
                .ThenBy(x => x.Name)
                .ToList();

            var dayQtyTotal = perProduct.Sum(x => x.Qty);
            var dayNetTotal = perProduct.Sum(x => x.NetSales);
            var dayVatTotal = perProduct.Sum(x => x.Vat);
            var dayGrossTotal = perProduct.Sum(x => x.GrossSales);

            var minCreated = items.Any() ? items.Min(x => x.CREATED_DATE) : (DateTime?)null;
            var maxCreated = items.Any() ? items.Max(x => x.CREATED_DATE) : (DateTime?)null;

            // Write a log entry that you can compare directly to the Product Mix report totals.
            // If your Notes column is small, consider truncating perProductLines or logging top N.
            try
            {
                var perProductLines = string.Join(
                    Environment.NewLine,
                    perProduct.Select(p =>
                        $"{p.PLU}\t{p.Name}\tQty={p.Qty:0.##}\tNetSales={p.NetSales:0.00}\tVAT={p.Vat:0.00}\tGross={p.GrossSales:0.00}")
                );

                var header =
                    $"Fourth Sales Reconciliation{Environment.NewLine}" +
                    $"Brand={brand?.brand_id} " +
                    $"Est={(establishment != null ? establishment.establishment_id.ToString() : "brand")} " +
                    $"MinCreated={(minCreated.HasValue ? minCreated.Value.ToString("yyyy-MM-dd HH:mm:ss") : "n/a")} " +
                    $"MaxCreated={(maxCreated.HasValue ? maxCreated.Value.ToString("yyyy-MM-dd HH:mm:ss") : "n/a")}{Environment.NewLine}" +
                    $"DAY TOTALS -> Qty={dayQtyTotal:0.##} Net(ProductMix)={dayNetTotal:0.00} VAT={dayVatTotal:0.00} Gross={dayGrossTotal:0.00}{Environment.NewLine}" +
                    $"PER PRODUCT:{Environment.NewLine}";

                //Logger.ScheduledTaskLogs.Add(new ScheduledTaskLog
                //{
                //    TaskName = "Fourth Sales Reconciliation",
                //    Notes = header + perProductLines
                //});

                //Logger.SaveChanges();
            }
            catch
            {
                // Never block submission due to logging
            }

            // ----------------------------
            // Build XML + submit
            // ----------------------------
            var fourthHeader = GenerateFourthHeaderForSales(items, brand, establishment);

            var xmlDoc = ConvertToXMLDoc(fourthHeader);
            XML = xmlDoc.ConvertXMLDocToString();

            var salesCallReturn = fhAPI.SubmitSales(this.LoginToken, xmlDoc);
            return salesCallReturn;
        }


        public static XmlDocument ConvertToXMLDoc(FourthHeader fourthHeader)
        {

            var XmlDoc = fourthHeader.ToXML();
            XmlDoc.DocumentElement.SetAttribute("xmlns", "");
            // Create an XML declaration. 

            new XmlStripper().RemoveAllNamespaces(XmlDoc.DocumentElement);
            /*var root = resultDoc.FirstChild;*/


            return XmlDoc;
        }




        public static XmlNode SerializeObjectToXmlNode(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Argument cannot be null");

            XmlNode resultNode = null;
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    xmlSerializer.Serialize(memoryStream, obj);
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
                memoryStream.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(memoryStream);

                resultNode = doc.DocumentElement;
            }

            return resultNode;
        }

        public static FourthHeader GenerateFourthHeaderForSales(
     IEnumerable<RevelSummedOrderItems> summedItems,
     Brand brand,
     Establishment est = null)
        {
            try
            {
                var items = (summedItems ?? Enumerable.Empty<RevelSummedOrderItems>()).ToList();

                // Build a PLU that is NEVER empty.
                // Priority:
                //  1) OrderItem SKU (if present and not "UNIDENTIFIED")
                //  2) PRODUCT_ID (numeric, stable)
                //  3) DBKEY_ID (numeric, stable)
                 string BuildPlu(RevelSummedOrderItems x)
                {
                    var sku = (x.SKU ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(sku) &&
                        !sku.Equals("UNIDENTIFIED", StringComparison.OrdinalIgnoreCase))
                    {
                        return sku;
                    }

                    if (x.PRODUCT_ID > 0)
                    {
                        return x.PRODUCT_ID.ToString();
                    }

                    // Last resort: use a stable unique id we have
                    return x.DBKEY_ID.ToString();
                }

                // Build a robust description even when Product lookup didn't help
                 string BuildDescription(RevelSummedOrderItems x)
                {
                    var name = (x.NAME ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(name)) return name;

                    var sku = (x.SKU ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(sku)) return sku;

                    if (x.PRODUCT_ID > 0) return $"Product {x.PRODUCT_ID}";

                    return "Unidentified Item";
                }

                // IMPORTANT:
                // - Do NOT filter out blank/UNIDENTIFIED SKUs anymore (we generate fallback PLUs).
                // - Group by the PLU we will actually submit (not just PRODUCT_ID).
                var salesItems = items
                    .Where(x => x != null)
                    .GroupBy(x => BuildPlu(x))
                    .Select(g =>
                    {
                        var qty = g.Sum(x => x.QUANTITY);

                // Product Mix "Total Sales" matches NET (ex VAT) => PURE_SALES
                var netTotal = g.Sum(x => x.PURE_SALES);

                // Keep VAT separate
                var vatTotal = g.Sum(x => x.TAX);

                // Gross totals (inc VAT)
                var grossTotal = g.Sum(x => x.PURE_SALES_PLUS_TAX);

                // Unit prices (so *Price fields are true unit prices, not totals)
                var netUnit = (qty == 0) ? 0m : netTotal / qty;
                        var grossUnit = (qty == 0) ? 0m : grossTotal / qty;

                // Pick a representative row for description
                var first = g
                            .OrderByDescending(x => !string.IsNullOrWhiteSpace(x.NAME))
                            .ThenByDescending(x => x.QUANTITY)
                            .First();

                        return new FourthHeaderOrganisationHeaderSalesHeaderSalesTransaction
                        {
                            PLU = g.Key,
                            Description = BuildDescription(first),

                            Quantity = qty,

                    // NET should match Product Mix
                    NetSalesPrice = netUnit,
                            TotalNetSales = netTotal,

                            VAT = vatTotal,

                    // Gross values (if Fourth wants them)
                    GrossSalesPrice = grossUnit,
                            TotalGrossSales = grossTotal
                        };
                    })
                    .ToList();

                FourthHeader fourthHeader = GenerateBaseFourthHeader(brand);
                PopulateFourthSalesHeader(items, brand, fourthHeader);

                // Transactions
                fourthHeader.OrganisationHeader[0].SalesHeader[0].SalesTransaction.AddRange(salesItems);

                // Establishment based location
                if (est != null)
                {
                    fourthHeader.OrganisationHeader[0].SalesHeader[0].Location = est.fourth_locationID;
                }

                return fourthHeader;
            }
            catch
            {
                throw;
            }
        }



        private static void PopulateFourthSalesHeader(IEnumerable<RevelSummedOrderItems> summedItems, Brand brand, FourthHeader fourthHeader)
        {
            //sales header
            fourthHeader.OrganisationHeader[0].SalesHeader = new List<FourthHeaderOrganisationHeaderSalesHeader>();
            fourthHeader.OrganisationHeader[0].SalesHeader.Add(new FourthHeaderOrganisationHeaderSalesHeader
            {
                SalesDate = (DateTime)summedItems.Min(x => x.CREATED_DATE),
                Location = brand.fourth_locationID,
                RevenueCentre = brand.fourth_RevenueCenter,
                SalesTransaction = new List<FourthHeaderOrganisationHeaderSalesHeaderSalesTransaction>()
            });
        }

        private static FourthHeader GenerateBaseFourthHeader(Brand brand)
        {
            var fourthHeader = new FourthHeader();
            fourthHeader.OrganisationHeader = new List<FourthHeaderOrganisationHeader>();

            fourthHeader.OrganisationHeader.Add(new FourthHeaderOrganisationHeader
            {
                OrganisationID = brand.fourth_locationID,
                UserName = brand.fourth_username,
                Password = brand.fourth_password,
            });
            return fourthHeader;
        }
    }
}
