using CsvHelper;
using Newtonsoft.Json;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Utility;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.SalesSummaryReport;
using shared.services.grind.railgunit.com.OpsReporting.SecondOpsReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using xero.railgunit.com.Grind.Utility;


namespace automatedreports.grind.railgunit.com.ScheduledTasks
{
    public class _VATRate_Cashup_RateReportJob : BaseJob
    {
        public async override Task Execute(IJobExecutionContext context)
        {
            var DatesToExclude = new List<DateTime>
            {
                //new DateTime(2020, 06, 18),
                //new DateTime(2020, 06, 23),
                //new DateTime(2020, 06, 26),
                //new DateTime(2020, 06, 27),
            };

            //DOESN@T INCLUDE FINAL DATE
            var startCycle = new DateTime(2021, 11, 01, 04,00,00);
            var endCycle = new DateTime(2021, 11, 29, 04, 00, 00);

            //var startCycle = DateTime.Now.AddDays(-1);
            //var endCycle = startCycle.AddDays(1);

            var dateSet = new List<DateTime>();
            var currentDate = startCycle;
            while (currentDate < endCycle)
            {
                if (!DatesToExclude.Contains(currentDate))
                {
                    dateSet.Add(currentDate);

                }
                currentDate = currentDate.AddDays(1);
                //build the set
            }

            //use the set

            foreach (var aDateToPushDatafor in dateSet)
            {
                //for TLS
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new
                RemoteCertificateValidationCallback
                (
                   delegate { return true; }
                );
                //MONDAY 3am - MONDAY 3am
                Console.WriteLine("VAT ops report job now starting");

                //var endDateRevel = new DateTime(2020, 06, 08, 04, 00, 00);
                //var endDateRevel = DateTime.Now.AddDays(-2); //get current time - take it back to MOnday, this fires Wednesday         
                //                                             //var endDateRevel = new DateTime(2019, 08, 26);
                //var endDate = new DateTime(endDateRevel.Year, endDateRevel.Month, endDateRevel.Day, 04, 00, 00);
                var endDate = aDateToPushDatafor.AddDays(1);
                var startDate = endDate.AddDays(-1); //get current time            
                Bootstrap();
                //set up establishments
                try
                {
                    Console.WriteLine(String.Format("NOW RUNNING RANGE: {0} to {1}", startDate, endDate));
                    db = new Revel._808nd.com.Models.GrindContext();
                    var establishments = db.Establishments.Where(x => x.establishment_id != 2).ToList();
                    //got data - now generate report for each store
                    List<VATRATEReportPOCO> secondOpsReportPOCO = new List<VATRATEReportPOCO>();

                    //using (var client = new GmailClient("emailnadz@gmail.com", "teenpunks23"))
                    //{
                    //    client.Send(new List<string> { "emailnadz@gmail.com" }, "Second ops report generating", "Working on it");
                    //}
                    foreach (var est in establishments)
                    {
                        Console.WriteLine("Now doing est:" + est.name);
                        //do ops report
                        //var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}", startDate.ToRevelDate(), endDate.ToRevelDate(), est.establishment_id);
                        var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}", startDate.ToRevelDate(), endDate.ToRevelDate(), est.establishment_id);

                        var response = await client.GetAsync(query);
                        var content = await response.Content.ReadAsStringAsync();
                        var poco = JsonConvert.DeserializeObject<RootObject>(content);

                        //get top level groups
                        var container = poco.CreateOperationsReportGroup();


                        //do sales summary report
                        var salesSummaryQuery = String.Format("https://shoreditchgrind.revelup.com/reports/sales_summary/json/?dining_option=5&employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}&format=json", startDate.ToRevelDate(), endDate.ToRevelDate(), est.establishment_id);
                        var salesResponse = await client.GetAsync(salesSummaryQuery);
                        var salesContent = await salesResponse.Content.ReadAsStringAsync();
                        var salesPoco = JsonConvert.DeserializeObject<List<RootObjectSalesSummary>>(salesContent).FirstOrDefault();

                        //now map data - inc sales report figures necessary

                        //old data
                        var dataComplete = MapOpsReportDataToVATReportPOCO(container, poco, est, salesPoco.other_total);
                        secondOpsReportPOCO.Add(dataComplete);

                        //new data
                        //var dataComplete = MapOpsReportDataToPOCOv2(container, poco, est, salesPoco.other_total);
                    }

                    //once data is complete, generate CSVS
                    //var itemsAsList = new List<SecondOpsReportPOCO> { item };
                    var weekEnd = endDate.AddDays(-1); //take one day back so sunday appears on W/E
                    var reportTitle = String.Format("VATRate Cash Up Journals for period {0} to {1}", startDate.ToString("dd_MM_yyyy"), endDate.ToString("dd_MM_yyyy"));
                    var reportName = reportTitle;
                    var fullFilePath = String.Format(@"c:\test\{0}.csv", reportName);
                    using (var writer = new StreamWriter(fullFilePath))
                    {
                        var csv = new CsvWriter(writer);
                        csv.WriteRecords(secondOpsReportPOCO);
                        csv.Flush();
                    }

                    ////now email
                    //using (var client = new GmailClient("grindandco808@gmail.com", "teenpunks23"))
                    //{

                    //    var listOfPeopleToEmail = new List<string> {
                    ////"michaela@grind.co.uk",
                    //"james.golding@grind.co.uk",
                    //"glynn@grind.co.uk",
                    //"emailnadz@gmail.com",                                   };

                    //    var toAttach = new List<Attachment> { new Attachment(fullFilePath) };
                    //    client.Send(listOfPeopleToEmail, reportName, "Please find attached. Regards", toAttach);

                    //}

                    Console.WriteLine("Job completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Use container where possible, otherwise use poco
        /// </summary>
        /// <param name="container"></param>
        /// <param name="poco"></param>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        private VATRATEReportPOCO MapOpsReportDataToVATReportPOCO(XeroOperationsProducClassGroupContainer container,
            RootObject poco,
            Establishment est,
            decimal appTotalSales)
        {
            var newPoco = new VATRATEReportPOCO();
            newPoco.EstablishmentId = est.establishment_id;
            newPoco.EstablishmentName = est.name;

            var taxData = poco.tax_data.ToList();

            //get objects
            var _20PercentData = taxData.Where(x => x.name == "20% VAT").ToList();
            var _5PercentData = taxData.Where(x => x.verbose_tax_rate == "5.000%" && x.name == "5% VAT").ToList(); 
            var _5PercentDataZeroRate = taxData.Where(x => x.verbose_tax_rate == "0.000%" && x.name == "5% VAT").ToList();
            var _PrevailingTax = taxData.Where(x => x.name == "Prevailing Tax").ToList();
            var _Untaxed = taxData.Where(x => x.name == "Untaxed").ToList(); ;
            var taxRoundingVariance = taxData.Where(x => x.name == "Tax Rounding Variance").ToList();

            //now map
            if (_20PercentData.Count > 0)
            {
                newPoco._20PercentVAT_Tax = _20PercentData.Sum(x => x.tax).ToString();
                newPoco._20PercentVAT_TaxableSales = _20PercentData.Sum(x=>x.taxable_sales).ToString();
                newPoco._20PercentVAT_TaxRate = _20PercentData.First().tax_rate.ToString();
            }
            if (_5PercentData.Count > 0)
            {
                newPoco._5PercentVAT_Tax = _5PercentData.Sum(x => x.tax).ToString();
                newPoco._5PercentVAT_TaxableSales = _5PercentData.Sum(x => x.taxable_sales).ToString();
                newPoco._5PercentVAT_TaxRate = _5PercentData.First().tax_rate.ToString();
            }

            if (_5PercentDataZeroRate.Count > 0)
            {
                newPoco._5PercentVATZeroRate_Tax = _5PercentDataZeroRate.Sum(x => x.tax).ToString();
                newPoco._5PercentVATZeroRate_TaxableSales = _5PercentDataZeroRate.Sum(x => x.taxable_sales).ToString();
                newPoco._5PercentVATZeroRate_TaxRate = _5PercentDataZeroRate.First().tax_rate.ToString();
            }
            if (_Untaxed.Count > 0)
            {
                newPoco._Untaxed_Tax = _Untaxed.Sum(x => x.tax).ToString();
                newPoco._Untaxed_TaxableSales = _Untaxed.Sum(x => x.taxable_sales).ToString();
                newPoco._Untaxed_TaxRate = _Untaxed.First().tax_rate.ToString();
            }

            if (_PrevailingTax.Count > 0)
            {
                newPoco._PrevailingTax_Tax = _PrevailingTax.Sum(x => x.tax).ToString();
                newPoco._PrevailingTax_TaxableSales = _PrevailingTax.Sum(x => x.taxable_sales).ToString();
                newPoco._PrevailingTax_TaxRate = _PrevailingTax.First().tax_rate.ToString();
            }
            //discounts            
            newPoco.Discounts = Convert.ToDecimal(container.GetTotalOrderDiscountSales() + container.GetTotalItemDiscountSales());

            //service fee
            newPoco.Untaxed_Service_Fee = Convert.ToDecimal(container.SalesData.service_fee_total);
            //tips
            newPoco.Tips = container.GetTips();
            //VAT
            newPoco.VAT = Convert.ToDecimal(container.SalesData.sales_tax); //CHECK CORRECT FIELD

            //House account
            newPoco.House_Account = Convert.ToDecimal(container.SalesData.house_account_payable) + Convert.ToDecimal(container.SalesData.house_account_receivable);
            //gift cards purchases
            newPoco.Gift_Card_Purchases = Convert.ToDecimal(container.SalesData.gift_sales_payable) + Convert.ToDecimal(container.SalesData.store_credit_sales_payable);
            //gift card used
            newPoco.Gift_Cards_Used = Convert.ToDecimal(container.SalesData.gift_sales_receivable) + Convert.ToDecimal(container.SalesData.store_credit_sales_receivable);

            //variance
            newPoco.Variance = 0.00M;
            //net to accoutn for 
            newPoco.Net_to_Account_For = Convert.ToDecimal(container.SalesData.net_account_for);

            //payments
            newPoco.Cash = Convert.ToDecimal(container.SalesData.cash_total);
            newPoco.Credit = Convert.ToDecimal(container.SalesData.credit_totals_with_refunds);
            newPoco.American_Express = Convert.ToDecimal(container.SalesData.american_express_totals_with_refunds);
            newPoco.App = appTotalSales;
            newPoco.MasterCard = Convert.ToDecimal(container.SalesData.mastercard_totals_with_refunds);
            newPoco.Visa = Convert.ToDecimal(container.SalesData.visa_totals_with_refunds);
            newPoco.OtherCredit = Convert.ToDecimal(container.SalesData.other_credit_card_totals_with_refunds);
            newPoco.Custom_Payment = Convert.ToDecimal(container.SalesData.custom_payment_totals_with_refunds);
            newPoco.Grand_Total = Convert.ToDecimal(container.SalesData.total_payments);

            newPoco.Payins = Convert.ToDecimal(container.SalesData.payouts_ins_total);
            newPoco.Payouts = Convert.ToDecimal(container.SalesData.payouts_outs_total);


            return newPoco;
        }
    }
}
