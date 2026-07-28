using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syncfusion.Linq;
using WebReboot.Grind._808nd.com.XMLConverters.Models;

namespace WebReboot.Grind._808nd.com.XMLConverters.XMLMappers
{
    public class RevelCsVToCaternetTillSalesMapper
    {

        public CaternetTillSales Map(DateTime salesDate, string tillServiceId, string tillUnitId, IEnumerable<CaternetCsvRow> revelCsvRow)
        {
            return new CaternetTillSales()
            {
                TradingDate = salesDate.ToString("yyyy-MM-dd"),
                TillServiceId = tillServiceId,
                TillUnitId = tillUnitId,
                Sales = Map(revelCsvRow)
            };

        }



        public List<Entry> Map(IEnumerable<CaternetCsvRow> revelCsvRows)
        {

            var entries = new List<Entry>();
            foreach (var row in revelCsvRows)
            {
                entries.Add(new Entry
                {
                    PLU = row.SKU,
                    Quantity = row.Quantity,
                    GrossSalesPrice =  row.GrossSalesPrice,
                    NetSalesPrice = row.NetSalesPrice,
                    TotalGrossSales = row.GrossSales,
                    TotalNetSales = row.NetSales,
                    Notes = row.Name,
                    TotalVAT = row.VAT,
                    SalesTypeRef = row.SalesTypeRef.ToString(),
                });

            }
            return entries;
        }
    }
}