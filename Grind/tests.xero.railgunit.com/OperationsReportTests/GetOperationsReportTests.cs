using AutoIt;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.OperationsReport.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xero.railgunit.com.Grind;
using xero.railgunit.com.Grind.Utility;


namespace tests.xero.railgunit.com.OperationsReportTests
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }

    [TestFixture]
    public class GetOperationsReportTests
    {
        DateTime start;
        DateTime end;
        HttpClient client;
        RevelClassTaxMappingService taxMappingService;
        RevelProductClassAccountMappingService accountMappingService;

        List<EstablishmentXeroMapping> grinds;
        string directoryPath = @"C:\ReveLCSVs\";
        string browserDownloadPath = @"C:\Users\n\Downloads\";


        [SetUp]
        public async Task Arrange()
        {
            DateTime dt = new DateTime(2018, 01, 01);

            start = new DateTime(dt.Year, dt.Month, dt.Day, 04, 00, 00);
            end = start.AddDays(7);

            var RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
            var RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];

            Establishment revOrg = new Establishment(1, "Grind",
       RevelAPIKEY,
       new Uri(RevelBaseURL));


            RevelFactory helperFactory = new RevelFactory(revOrg);
            client = helperFactory.CreateShoreditchGrindHttpClient(RevelBaseURL, RevelAPIKEY);

            taxMappingService = new RevelClassTaxMappingService();
            accountMappingService = new RevelProductClassAccountMappingService();
            grinds = OtherExtensions.GetAllGrindStoresForReportDownload().ToList();
        }


        public async Task Act()
        {


        }




        [Test]
        public async Task Should_Get_Orderoperations_Report_For_Date_Range_and_Establishment()
        {


            //this mon
            // DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            DateTime dt = new DateTime(2018, 01, 01);

            var start = new DateTime(dt.Year, dt.Month, dt.Day, 04, 00, 00);
            var end = start.AddDays(7);

            //if (!Directory.Exists(directoryPath))
            //{
            //    Directory.CreateDirectory(directoryPath);
            //}
            //else
            //{
            //    //clean all files out of dir before you make a run
            //    DirectoryInfo dir = new DirectoryInfo(directoryPath);
            //    foreach (FileInfo fi in dir.GetFiles())
            //    {
            //        fi.Delete();
            //    }

            //}
            ////INIT
            ////go to Grind
            //SUT.Navigate().GoToUrl("https://shoreditchgrind.revelup.com/");

            ////log in
            //SUT.FindElement(By.Id("id_username")).SendKeys("nadavdrewe@gmail.com");
            //SUT.FindElement(By.Id("id_password")).SendKeys("Diagonal23");
            ////click
            //SUT.FindElement(By.ClassName("css3")).Click();


            //Operations loop
            foreach (var grindName in grinds)
            {
                //string saveName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                ////LOOP START
                ////go to mix
                //SUT.Navigate().GoToUrl("https://shoreditchgrind.revelup.com/reports/operations/");

                //Thread.Sleep(4000);
                ////open branch menu
                //SUT.FindElement(By.ClassName("location")).Click();
                //Thread.Sleep(4000);
                ////click shoreditch grind
                //SUT.FindElement(By.XPath(String.Format("//*[contains(text(), '{0}')]", grindName.XeroContactName))).Click();
                //Thread.Sleep(4000);

                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = JsonConvert.DeserializeObject<RootObject>(content);


                //SUT.Navigate().GoToUrl(query);

                //SUT.SwitchTo().Window(SUT.Title)




                //     .Manage().Window.Maximize();



                //GET RESPONSE BODY AS TEXT STRING AND SAVE IT SUING FILE.SAVE

                //SendKeys.SendWait("^(s)");
                //SendKeys.SendWait("{ENTER}");
                //Thread.Sleep(3000);
                //SendKeys.SendWait(Path.Combine(directoryPath, saveName));
                //SendKeys.SendWait("{ENTER}");

                //AutoItX.Send("{CTRLDOWN}s{CTRLUP}");
                //AutoItX.Send("{Enter}");
                //Thread.Sleep(3000);
                //AutoItX.Send(Path.Combine(directoryPath, saveName));
                //AutoItX.Send("{Enter}");

            }


            //do tests
            foreach (var grindName in grinds)
            {
                var fullGrindName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                try
                {
                    var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                    var response = await client.GetAsync(query);
                    var content = await response.Content.ReadAsStringAsync();
                    var poco = JsonConvert.DeserializeObject<RootObject>(content);

                    poco.ShouldNotBeNull();
                }
                catch (Exception ex)
                {

                    throw;
                }



            }


        }



        [Test]
        public async Task Should_Get_Troc_Tips_For_Date_Range_and_Establishment()
        {
            //do tests
            foreach (var grindName in grinds)
            {
                var fullGrindName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                try
                {
                    var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                    var response = await client.GetAsync(query);
                    var content = await response.Content.ReadAsStringAsync();
                    var poco = JsonConvert.DeserializeObject<RootObject>(content);


                    //232.19
                    var container = poco.CreateOperationsReportGroup();
                    var tips = Convert.ToDecimal(grindName.XeroContactName.Contains("Soho") ? container.GetTips() : container.GetServiceFee());

                    try
                    {
                        tips.ShouldBeGreaterThan(0);
                    }
                    catch (Exception)
                    {

                        Console.WriteLine(fullGrindName + " has zero tips or service charge");
                    }

                }
                catch (Exception ex)
                {

                    throw;
                }



            }


        }



        [Test]
        public async Task Should_Get_StoreCredit_And_Gift_Credit_For_Date_Range_and_Establishment()
        {
            //do tests
            foreach (var grindName in grinds)
            {
                var fullGrindName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = JsonConvert.DeserializeObject<RootObject>(content);


                //get some tips
                var container = poco.CreateOperationsReportGroup();
                var payable = container.GetGiftAndServicePayable();

                try
                {
                    payable.ShouldBeGreaterThan(0);

                }
                catch (Exception ex)
                {

                    Console.WriteLine(fullGrindName + " has zero store credit or service charge");
                }



            }

        }


        [Test]
        public async Task Should_Get_Parent_Categories_From_Data()
        {

            foreach (var grindName in grinds)
            {
                var fullGrindName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                try
                {
                    var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                    var response = await client.GetAsync(query);
                    var content = await response.Content.ReadAsStringAsync();
                    var poco = JsonConvert.DeserializeObject<RootObject>(content);


                    var allStandardCats = "";
                    var allParentCats = poco.product_mix_data.Select(x => x.parent_pclass).Distinct().ToList();

                    allParentCats.Count().ShouldBeGreaterThan(0);

                }
                catch (Exception ex)
                {

                    throw;
                }

            }


        }

        [Test]
        public async Task Should_Correctly_Convert_RootMix_To_Grouping()
        {
            foreach (var grindName in grinds)
            {
                var fullGrindName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = JsonConvert.DeserializeObject<RootObject>(content);

                var allParentCats = poco.product_mix_data.Select(x => x.parent_pclass).Distinct().ToList();
                var groupings = poco.CreateOperationsReportGroup();

                //check there are all the groups
                allParentCats.ForEach(x =>
                {

                    var test = groupings.XeroOperationsProducClassGroups.FirstOrDefault(y => y.ParentCategoryName == x);
                    test.ShouldNotBeNull();
                });

            }



        }

        [Test]
        public async Task Should_Correctly_Convert_RootMix_To_Grouping_And_All_Stores_Should_Have_Sales_In_Every_Category()
        {
            foreach (var grindName in grinds)
            {
                var fullGrindName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = JsonConvert.DeserializeObject<RootObject>(content);


                var allParentCats = poco.product_mix_data.Select(x => x.parent_pclass).Distinct().ToList();
                var groupings = poco.CreateOperationsReportGroup();

                //check there are all the groups
                allParentCats.ForEach(x =>
                {
                    var name = grindName;
                    var test = groupings.XeroOperationsProducClassGroups.FirstOrDefault(y => y.ParentCategoryName == x);
                    test.ShouldNotBeNull();
                    var sales = test.GetTotalTaxedSales();
                    var tax = test.GetTotalTaxAmount();

                    try
                    {
                        sales.ShouldBeGreaterThan(0);
                        tax.ShouldBeGreaterThan(0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(name + " has a cat with zero sum: " + test.ParentCategoryName);
                    }
                });

            }



        }






        [Test]
        public async Task Should_Match_Extension_Method_Results_With_Selected_Results()
        {
            foreach (var grindName in grinds)
            {
                var fullGrindName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = JsonConvert.DeserializeObject<RootObject>(content);



                var container = poco.CreateOperationsReportGroup();
                //check against inital data
                container.XeroOperationsProducClassGroups.ToList().ForEach(x =>
                {

                    x.GetItemDiscounts().ShouldBe(x.ProductMix.discount);
                    x.GetOrderDiscounts().ShouldBe(Convert.ToDecimal(x.ProductMix.order_discount));

                    x.GetTotalTaxedSales();
                    x.GetTotalNonTaxedSales();
                    x.GetTotalTaxAmount();

                });


            }



        }


        [Test]
        public async Task Should_Assign_Account_Codes_To_Each_Category()
        {
            foreach (var grindName in grinds)
            {
                var fullGrindName = OtherExtensions.GetGrindName(grindName.XeroContactName);

                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, grindName.EstablishmentId);
                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = JsonConvert.DeserializeObject<RootObject>(content);




                var container = poco.CreateOperationsReportGroup();
                //check against inital data
                container.XeroOperationsProducClassGroups.ToList().ForEach(x =>
                {
                    var cuurentAccount = x.ParentCategoryName;
                    try
                    {
                        var accountCode = accountMappingService.GetRevelAccountCodeForCategory(x.ParentCategoryName).AccountCode;
                        accountCode.ShouldNotBeNullOrWhiteSpace();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                });
            }




        }


    }
}
