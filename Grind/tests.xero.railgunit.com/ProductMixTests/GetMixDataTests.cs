using Newtonsoft.Json;
using NUnit.Framework;
using Revel._808nd.com.ProductMix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Grind;
using Shouldly;
using xero.railgunit.com.Grind.Extension;

namespace tests.xero.railgunit.com.ProductMixTests
{
    [TestFixture]
    public class GetMixDataTests
    {

        RootObject productMixRoot;
        string JSONMixString = "";
        const string filePath = @"C:\test\json\mix.json";
        RevelClassTaxMappingService taxMappingService;
        RevelProductClassAccountMappingService accountMappingService;


        [SetUp]
        public async Task Arrange()
        {
            taxMappingService = new RevelClassTaxMappingService();
            accountMappingService = new RevelProductClassAccountMappingService();
            JSONMixString = File.ReadAllText(filePath);
            productMixRoot = JsonConvert.DeserializeObject<RootObject>(JSONMixString);
            await Act();
        }

        public async Task Act()
        {


        }

        [Test]
        public async Task Should_Get_Data_From_File()
        {
            JSONMixString.ShouldNotBeNullOrWhiteSpace();
            productMixRoot.ShouldNotBeNull();

        }

        [Test]
        public async Task Should_Get_Parent_Categories_From_Data()
        {

            var allStandardCats = "";
            var allParentCats = productMixRoot.productmix.Select(x => x.parent_pclass).Distinct().ToList();

            allParentCats.Count().ShouldBeGreaterThan(0);

        }

        [Test]
        public async Task Should_Correctly_Convert_RootMix_To_Grouping()
        {
            var groupings = productMixRoot.GetParentCategoriesAndProductGroups();
            //check against inital data
            groupings.ToList().ForEach(x =>
            {
                x.ProductMixes.ToList().ForEach(y =>
                {
                    var item = productMixRoot.productmix.First(z => z.product_name == y.product_name);
                    item.price.ShouldBe(y.price);
                    item.order_discount.ShouldBe(y.order_discount);
                    item.product_name.ShouldBe(y.product_name);
                    item.taxable_sales.ShouldBe(y.taxable_sales);
                    item.untaxable_sales.ShouldBe(y.untaxable_sales);

                });
            });

        }

        [Test]
        public async Task Should_Match_Extension_Method_Results_With_Selected_Results()
        {
            var groupings = productMixRoot.GetParentCategoriesAndProductGroups();
            //check against inital data
            groupings.ToList().ForEach(x =>
            {

                x.GetItemDiscounts().ShouldBe(x.ProductMixes.Sum(y => Convert.ToDecimal(y.discount)));
                x.GetOrderDiscounts().ShouldBe(x.ProductMixes.Sum(y => Convert.ToDecimal(y.order_discount)));

                x.GetTotalComps().ShouldBe(x.ProductMixes.Sum(y => Convert.ToInt16(y.n_comps)));
                x.GetTotalQty().ShouldBe(x.ProductMixes.Sum(y => Convert.ToInt16(y.n_items)));
                x.GetTotalVoids().ShouldBe(x.ProductMixes.Sum(y => Convert.ToInt16(y.n_voids)));

                x.GetTotalTaxedSales().ShouldBe(x.ProductMixes.Sum(y => Convert.ToDecimal(y.taxable_sales)));
                x.GetTotalNonTaxedSales().ShouldBe(x.ProductMixes.Sum(y => Convert.ToDecimal(y.untaxable_sales)));
                x.GetTotalTaxAmount().ShouldBe(x.ProductMixes.Sum(y => Convert.ToDecimal(y.tax)));

            });

        }


        [Test]
        public async Task Should_Assign_Correct_Account_Codes_To_Each_Category()
        {
            var groupings = productMixRoot.GetParentCategoriesAndProductGroups();
            //check against inital data
            groupings.ToList().ForEach(x =>
            {
                var accountCode = accountMappingService.GetRevelAccountCodeForCategory(x.ParentCategoryName).AccountCode;
                accountCode.ShouldNotBeNullOrWhiteSpace();
            });

        }



    }
}
