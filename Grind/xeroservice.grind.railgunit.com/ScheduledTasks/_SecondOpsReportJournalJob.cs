using CsvHelper;
using Newtonsoft.Json;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Utility;
using Revel._808nd.com.OperationsReport.Factory;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.SalesSummaryReport;
using shared.services.grind.railgunit.com.OpsReporting.SecondOpsReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Grind;
using xero.railgunit.com.Grind.Utility;

namespace xeroservice.grind.railgunit.com.ScheduledTasks
{

    [Obsolete("The correct one is in automatedreports.grind.railgunit.com")]
    public class _SecondOpsReportJournalJob : BaseJob
    {
        public async override Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Second ops report job now starting");
            //var endDate = DateTime.Now; //get current time            
            var endDate = new DateTime(2018, 08, 27, 04, 00, 00);
            var startDate = endDate.AddDays(-7); //get current time            
            Bootstrap();
            //set up establishments
            try
            {
                db = new Revel._808nd.com.Models.GrindContext();
                var establishments = db.Establishments.Where(x => x.establishment_id != 2).ToList();
                //got data - now generate report for each store
                List<SecondOpsReportPOCO> secondOpsReportPOCO = new List<SecondOpsReportPOCO>();
                foreach (var est in establishments)
                {
                    //do ops report
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
                    var dataComplete = MapOpsReportDataToPOCO(container, poco, est, salesPoco.other_total);
                    secondOpsReportPOCO.Add(dataComplete);
                }

                //once data is complete, generate CSVS
                //var itemsAsList = new List<SecondOpsReportPOCO> { item };
                var reportName = string.Format("V2OpsReport_{0}", startDate.ToString("dd_MM_yyyy"));//, item.EstablishmentName);
                var fullFilePath = String.Format(@"c:\test\{0}.csv", reportName);
                using (var writer = new StreamWriter(fullFilePath))
                {
                    var csv = new CsvWriter(writer);
                    csv.WriteRecords(secondOpsReportPOCO);
                    csv.Flush();
                }


                Console.WriteLine("Job completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Use container where possible, otherwise use poco
        /// </summary>
        /// <param name="container"></param>
        /// <param name="poco"></param>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        private SecondOpsReportPOCO MapOpsReportDataToPOCO(XeroOperationsProducClassGroupContainer container, RootObject poco, Establishment est, decimal appTotalSales)
        {
            var newPoco = new SecondOpsReportPOCO();
            newPoco.EstablishmentId = est.establishment_id;
            newPoco.EstablishmentName = est.name;

            //cats            
            newPoco.Bar += poco.product_mix_data.Where(x => x.product_name == "Bar inc. Soft and Juice").FirstOrDefault()?.price ?? 0.00M;
            newPoco.BarTaxable += poco.product_mix_data.Where(x => x.product_name == "Bar inc. Soft and Juice").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.BarUntaxed += poco.product_mix_data.Where(x => x.product_name == "Bar inc. Soft and Juice").FirstOrDefault()?.untaxable_sales ?? 0.00M;

            //this is to bundle old cats
            newPoco.Bar += poco.product_mix_data.Where(x => x.product_name == "Bar").FirstOrDefault()?.price ?? 0.00M;
            newPoco.BarTaxable += poco.product_mix_data.Where(x => x.product_name == "Bar").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.BarUntaxed += poco.product_mix_data.Where(x => x.product_name == "Bar").FirstOrDefault()?.untaxable_sales ?? 0.00M;

            newPoco.Bar += poco.product_mix_data.Where(x => x.product_name == "Juice").FirstOrDefault()?.price ?? 0.00M;
            newPoco.BarTaxable += poco.product_mix_data.Where(x => x.product_name == "Juice").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.BarUntaxed = poco.product_mix_data.Where(x => x.product_name == "Juice").FirstOrDefault()?.untaxable_sales ?? 0.00M;

            newPoco.Bar += poco.product_mix_data.Where(x => x.product_name == "Soft Drinks").FirstOrDefault()?.price ?? 0.00M;
            newPoco.BarTaxable += poco.product_mix_data.Where(x => x.product_name == "Soft Drinks").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.BarUntaxed += poco.product_mix_data.Where(x => x.product_name == "Soft Drinks").FirstOrDefault()?.untaxable_sales ?? 0.00M;
            //end old cats

            newPoco.Coffee_Hot_Drinks = poco.product_mix_data.Where(x => x.product_name == "Coffee/Hot Drinks").FirstOrDefault()?.price ?? 0.00M;
            newPoco.Coffee_Hot_DrinksTaxable = poco.product_mix_data.Where(x => x.product_name == "Coffee/Hot Drinks").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.Coffee_Hot_DrinksUntaxed = poco.product_mix_data.Where(x => x.product_name == "Coffee/Hot Drinks").FirstOrDefault()?.untaxable_sales ?? 0.00M;

            newPoco.Food = poco.product_mix_data.Where(x => x.product_name == "Food").FirstOrDefault()?.price ?? 0.00M;
            newPoco.FoodTaxable = poco.product_mix_data.Where(x => x.product_name == "Food").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.FoodUntaxed = poco.product_mix_data.Where(x => x.product_name == "Food").FirstOrDefault()?.untaxable_sales ?? 0.00M;

            newPoco.Retail = poco.product_mix_data.Where(x => x.product_name == "Retail").FirstOrDefault()?.price ?? 0.00M;
            newPoco.RetailTaxable = poco.product_mix_data.Where(x => x.product_name == "Retail").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.RetailUntaxed = poco.product_mix_data.Where(x => x.product_name == "Retail").FirstOrDefault()?.untaxable_sales ?? 0.00M;

            newPoco.Extra_Items = poco.product_mix_data.Where(x => x.product_name == "Extra Items").FirstOrDefault()?.price ?? 0.00M;
            newPoco.Extra_ItemsTaxable = poco.product_mix_data.Where(x => x.product_name == "Extra Items").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.Extra_ItemsUntaxed = poco.product_mix_data.Where(x => x.product_name == "Extra Items").FirstOrDefault()?.untaxable_sales ?? 0.00M;

            newPoco.Unknown_ClassTaxable = poco.product_mix_data.Where(x => x.product_name == "Unknown Class").FirstOrDefault()?.price ?? 0.00M;
            newPoco.Unknown_ClassTaxable = poco.product_mix_data.Where(x => x.product_name == "Unknown Class").FirstOrDefault()?.taxable_sales ?? 0.00M;
            newPoco.Unknown_ClassUntaxed = poco.product_mix_data.Where(x => x.product_name == "Unknown Class").FirstOrDefault()?.untaxable_sales ?? 0.00M;


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
