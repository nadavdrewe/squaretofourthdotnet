using AutoIt;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Revel._808nd.com.OperationsReport.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tests.xero.railgunit.com.SeleniumTests
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }

    public static class Extensions
    {


    }

    [TestFixture]
    public class BrowserDownloadTests
    {


        IWebDriver SUT;
        List<string> grinds;
        string directoryPath = @"C:\ReveLCSVs\";
        string browserDownloadPath = @"C:\Users\n\Downloads\";


        [SetUp]
        public async Task Arrange()
        {
            SUT = new ChromeDriver();
            grinds = new List<string> {
                "Shoreditch",
                "Soho",
                "London Grind",
                "Royal Exchange",
                "Covent Garden",
                "Clerkenwell",
                "Whitechapel",
                "Exmouth"
                            };
        }


        public async Task Act()
        {


        }


        [Test]
        public async Task Should_Download_Json_For_All_Stores()
        {

            //this mon
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            var start = new DateTime(dt.Year, dt.Month, dt.Day, 04, 00, 00);
            var end = start.AddDays(7);

            //last mon
            var test = DateTime.Now.AddDays(-2).StartOfWeek(DayOfWeek.Monday);
            //check for existing directories

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            else
            {
                //clean all files out of dir before you make a run
                DirectoryInfo dir = new DirectoryInfo(directoryPath);
                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }

            }
            //INIT
            //go to Grind
            SUT.Navigate().GoToUrl("https://shoreditchgrind.revelup.com/");

            //log in
            SUT.FindElement(By.Id("id_username")).SendKeys("nadavdrewe@gmail.com");
            SUT.FindElement(By.Id("id_password")).SendKeys("Diagonal23");
            //click
            SUT.FindElement(By.ClassName("css3")).Click();

            ////product mix loop
            //foreach (var grindName in grinds)
            //{


            //    //LOOP START
            //    //go to mix
            //    SUT.Navigate().GoToUrl(String.Format("https://shoreditchgrind.revelup.com/reports/product_mix/"));

            //    Thread.Sleep(5000);
            //    //open branch menu
            //    SUT.FindElement(By.ClassName("location")).Click();
            //    Thread.Sleep(5000);
            //    //click shoreditch grind
            //    SUT.FindElement(By.XPath(String.Format("//*[contains(text(), '{0}')]", grindName))).Click();
            //    Thread.Sleep(5000);

            //    var testString = String.Format("https://shoreditchgrind.revelup.com/reports/product_mix/data/?sort_by=&sort_reverse=&combo_expand=&employee=&online_app=&online_app_type=&online_app_platform=&dining_option=&show_opened=1&show_unpaid=1&show_irregular=1&sort_view=0&show_product=1&show_sku=1&show_class=1&quantity_settings=3&taxable_not_taxable=1&item_discount=1&order_discount=1&tax_column=1&no-filter=0&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&format=json", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year);

            //    SUT.Navigate().GoToUrl(testString);

            //AutoItX.Send("{CTRLDOWN}s{CTRLUP}");
            //AutoItX.Send("{Enter}");

            //Thread.Sleep(3000);

            //var name = grindName.Replace(' ', '_');
            //AutoItX.Send(Path.Combine(direttoryPath, String.Format("{0}_ProdcutMix.json", name)));
            ////AutoItX.WinActivate(@"");
            //AutoItX.Send("{Enter}");

            //}


            //Operations loop
            foreach (var grindName in grinds)
            {

                //LOOP START
                //go to mix
                SUT.Navigate().GoToUrl("https://shoreditchgrind.revelup.com/reports/operations/");

                Thread.Sleep(4000);
                //open branch menu
                SUT.FindElement(By.ClassName("location")).Click();
                Thread.Sleep(4000);
                //click shoreditch grind
                SUT.FindElement(By.XPath(String.Format("//*[contains(text(), '{0}')]", grindName))).Click();
                Thread.Sleep(4000);

                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year);
                SUT.Navigate().GoToUrl(query);

                AutoItX.Send("{CTRLDOWN}s{CTRLUP}");
                AutoItX.Send("{Enter}");

                Thread.Sleep(3000);
                var name = grindName.Replace(' ', '_');
                AutoItX.Send(Path.Combine(directoryPath, String.Format("{0}_Operations.json", name)));
                AutoItX.Send("{Enter}");

            }

            SUT.Close();

        }





        [Test]
        public async Task Should_Download_Ops_Report_For_All_Stores()
        {

            //INIT
            //go to Grind
            SUT.Navigate().GoToUrl("https://shoreditchgrind.revelup.com/");

            //log in
            SUT.FindElement(By.Id("id_username")).SendKeys("nadavdrewe@gmail.com");
            SUT.FindElement(By.Id("id_password")).SendKeys("Diagonal23");
            //click
            SUT.FindElement(By.ClassName("css3")).Click();



            foreach (var grindName in grinds)
            {


                //LOOP START
                //go to mix
                SUT.Navigate().GoToUrl("https://shoreditchgrind.revelup.com/reports/operations/");

                Thread.Sleep(4000);
                //open branch menu
                SUT.FindElement(By.ClassName("location")).Click();
                Thread.Sleep(4000);
                //click shoreditch grind
                SUT.FindElement(By.XPath(String.Format("//*[contains(text(), '{0}')]", grindName))).Click();
                Thread.Sleep(4000);
                SUT.Navigate().GoToUrl("https://shoreditchgrind.revelup.com/reports/operations/");


                Thread.Sleep(4000);
                SUT.FindElement(By.XPath(String.Format("//*[contains(text(), '⋯')]", grindName))).Click();

                Thread.Sleep(4000);
                SUT.FindElement(By.XPath(String.Format("//*[contains(text(), 'CSV')]", grindName))).Click();
                Thread.Sleep(4000);


                //renmame files - make sure you set chrome default save directory to what it needs to be!!
                new DirectoryInfo(@"C:\Users\n\Downloads").GetFiles()
                                                  .OrderByDescending(f => f.LastWriteTime)
                                                  .ToList().First().MoveTo((String.Format(@"C:\test\grinds\{0}_Operations.csv", grindName)));



            }
            SUT.Close();

        }


        [Test]
        public async Task Should_Be_Able_To_Convert_Json_To_POCO()
        {
            var allFiles = new DirectoryInfo(directoryPath).GetFiles()
                                               .OrderByDescending(f => f.LastWriteTime)
                                               .ToList();

            foreach (var item in allFiles)
            {
                try
                {

                    var fileString = File.ReadAllText(item.FullName);
                    var poco = JsonConvert.DeserializeObject<RootObject>(fileString);
                    poco.ShouldNotBeNull();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

    }
}
