using Newtonsoft.Json;
using NUnit.Framework;
using Revel._808nd.com.CaternetData.Models;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Revel._808nd.com.ProductMix;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;



namespace UnitTestProject1.ProductMixTests
{
    [TestFixture]
    public class ProductMixTests
    {
        HttpClient client { get; set; }
        private GrindContext db = new GrindContext(); //get existing values
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        private WebserviceDataWriter SUT;

        private string result;

        [SetUp]
        public async Task Given()
        {
            client = new HttpClient();

        }


        //tests
        [Test]
        public async Task Should_Get_Product_Mix_For_Date_Range_and_Establishment()
        {
            client.Timeout = TimeSpan.FromSeconds(600);

            var baseaddressSTring = @"https://shoreditchgrind.revelup.com/";
            client.BaseAddress = new Uri(baseaddressSTring);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("API-AUTHENTICATION", "408d6c05f2864ece90c037333d64f333:9ae943831e7f443b9edf3a6203e66598290fc7d2f3244ca9b69dd67404aa39f2");
            client.DefaultRequestHeaders.Add("Referer", baseaddressSTring);

            var start = new DateTime(2018, 06, 01);
            var end = new DateTime(2018, 06, 03);
            var est = 1;

            var query = ProductMixQueryFactory.GenerateDateRangeQuery(start, end, est);
            var result = await client.GetAsync(query);
            var content = await result.Content.ReadAsStringAsync();

            var res = JsonConvert.DeserializeObject<ProductMixRootObject>(content);

            var allParentClasses = res.product_classes.Select(x => x.parent_class_name).Distinct();

            var compelte = "";
        }

        [Test]
        public async Task Should_Get_Product_Mix_For_Date_Range_and_Establishment_Anc_Convert_To_Caternet_Rows()
        {
            client.Timeout = TimeSpan.FromSeconds(600);

            var baseaddressSTring = @"https://shoreditchgrind.revelup.com/";
            client.BaseAddress = new Uri(baseaddressSTring);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("API-AUTHENTICATION", "408d6c05f2864ece90c037333d64f333:9ae943831e7f443b9edf3a6203e66598290fc7d2f3244ca9b69dd67404aa39f2");
            client.DefaultRequestHeaders.Add("Referer", baseaddressSTring);

            var start = new DateTime(2018, 06, 05);
            var end = new DateTime(2018, 06, 06);
            var est = 1;

            var query = ProductMixQueryFactory.GenerateDateRangeQuery(start, end, est);
            var result = await client.GetAsync(query);
            var content = await result.Content.ReadAsStringAsync();

            var res = JsonConvert.DeserializeObject<ProductMixRootObject>(content);

            var allParentClasses = res.product_classes.Select(x => x.parent_class_name).Distinct();

            //convert to caternet rows
            var rowTypes = res.productmix.Select(x => x.row_type).Distinct().ToList();
            var prodMis = res.productmix.Where(x => x.row_type == "Product" || (x.row_type == "Parent_Product" && !String.IsNullOrWhiteSpace(x.product_sku))).OrderBy(x => x.product_name).ToList();

            db = new GrindContext("GrindLiveContext");
            var caternetRows = new List<CaternetCsvRow>();
            List<CaternetCsvRow> rows = new List<CaternetCsvRow>();

            prodMis.ForEach(x =>
            {

                var prodInDb = db.Products.FirstOrDefault(X => X.sku.Trim() == x.product_sku.Trim());

                var comps = Convert.ToInt32(x.n_comps);
                var voids = Convert.ToInt32(x.n_voids);
                var qty = Convert.ToInt32(x.n_items);
                var totalQty = qty + comps - voids;

                var name = x.product_name;
                var pClass = x.product_class;
                var sku = x.product_sku;
                var price = prodInDb?.price ?? 0;
                var tax = x.tax;
                var totalInDisc = x.gm;



                var row = new CaternetCsvRow
                {
                    Quantity = totalQty,
                    GrossSalesPrice = price,
                    SKU = sku,
                    VAT = Convert.ToDecimal(tax),
                    Name = name,
                    GrossSales = Convert.ToDecimal(totalInDisc),
                    SalesTypeRef = 0
                };

                rows.Add(row);
            



            });

            rows.Count().ShouldBe(prodMis.Count());

        }




    }
}

