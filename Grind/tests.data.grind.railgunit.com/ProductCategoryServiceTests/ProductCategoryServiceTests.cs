using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.ServiceImplemenations;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using System.Data.SqlClient;
using System.Data;
using Revel._808nd.com.Classes.Reporting.Caternet;

namespace tests.data.grind.railgunit.com
{

    /// <summary>
    /// We using live data - course. Quick and dirty bro.
    /// </summary>
    [TestFixture]
    public class ProductCategoryServiceTests
    {

        GrindContext db;
        List<ProductCategory> allCatss;
        ProductCategoryService sut;
        ProductClassService prodClassService;
        Product product;
        List<Product> somePRods = new List<Product>();

        //arrange
        [SetUp]
        public void given_all_revel_categories()
        {

            // somePRods = db.Products.OrderByDescending(x => x.product_id).Take(100).ToList();
            db = new GrindContext();
            allCatss = db.ProductCategories.ToList();
            product = db.Products.FirstOrDefault(x => x.resource_uri == "/resources/Product/18284/");
            somePRods = db.Products.Where(x => x.active == "True").OrderByDescending(x => x.product_id).Take(1000).ToList();
            sut = new ProductCategoryService(allCatss);


        }

        //arrange
        public void when_given_a_product()
        {

        }

        [Test]
        public void should_return_expected_parent_categories()
        {

            try
            {

                var result = sut.GetParentCategory(product);
                result.ShouldNotBeNull();

                foreach (var item in somePRods)
                {
                    var result2 = sut.GetParentCategory(item);
                    result2.ShouldNotBeNull();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        [Test]
        public void should_get__expected_parent_classes_for_item_summaries()
        {
            var allclasses = db.ProductClasses.ToList();
            var allParentCats = new List<CaternetItemSummaryCategory>();
            var prodsWithNoClass = new List<Product>();
            try
            {
                foreach (var est in db.Establishments.Where(x => x.establishment_id != 2).ToList())
                {
                    var syncStart = new DateTime(2017, 11, 08, 03, 00, 00);
                    var syncEnd = syncStart.AddDays(6);

                    db = new GrindContext();
                    var SUT2 = new CaternetItemSummaryService(db);
                    var result3 = SUT2.GetSummaryForDateRange(syncStart, syncEnd, est.establishment_id);

                    result3.ShouldNotBeEmpty();

                    var productIdsToTestFOrDay = result3.Select(x => x.ProductId).ToList();
                    foreach (var id in productIdsToTestFOrDay)
                    {
                        var prod = db.Products.FirstOrDefault(x => x.product_id == id);
                        if (String.IsNullOrEmpty(prod.productclass))
                        {
                            prodsWithNoClass.Add(prod);
                        }
                        else {
                            var result = ProductClassService.GetParentRootClass(prod, allclasses);
                            result.ShouldNotBeNull();

                            if (allParentCats.FirstOrDefault(x => x.Name == result.name) == null)
                            {
                                allParentCats.Add(new CaternetItemSummaryCategory
                                {
                                    Id = result.id,
                                    Name = result.name,
                                    Summaries = new List<CaternetItemSummary> { result3.First(x => x.ProductId == prod.product_id) }
                                });
                            }
                            else
                            {
                                //todo: complete
                                //FINISH THIS
                                var oneWeWant = allParentCats.FirstOrDefault(x => x.Name == result.name).Summaries;
                                //get 
                            }
                        }
                       
                    }
                }


                var allCastsComplete = "";

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        [Test]
        public void should_get__expected_parent_categories_for_item_summaries()
        {
            var allParentCats = new List<CaternetItemSummaryParentCategory>();
            try
            {
                foreach (var est in db.Establishments.Where(x => x.establishment_id != 2).ToList())
                {
                    var syncStart = new DateTime(2017, 11, 08, 03, 00, 00);
                    var syncEnd = syncStart.AddDays(6);

                    db = new GrindContext();
                    var SUT2 = new CaternetItemSummaryService(db);
                    var result3 = SUT2.GetSummaryForDateRange(syncStart, syncEnd, est.establishment_id);

                    result3.ShouldNotBeEmpty();

                    var productIdsToTestFOrDay = result3.Select(x => x.ProductId).ToList();

                    foreach (var id in productIdsToTestFOrDay)
                    {
                        var prod = db.Products.FirstOrDefault(x => x.product_id == id);
                        var result = sut.GetParentCategory(prod);
                        result.ShouldNotBeNull();

                        if (allParentCats.FirstOrDefault(x => x.Name == result.name) == null)
                        {
                            allParentCats.Add(new CaternetItemSummaryParentCategory
                            {
                                //Id = result.productcategory_id,
                                //Name = result.name

                            });
                        }

                    }


                }


                var allCastsComplete = "";

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

}

