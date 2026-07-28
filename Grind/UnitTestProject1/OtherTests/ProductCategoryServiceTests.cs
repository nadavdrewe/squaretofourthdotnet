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

namespace tests.xero.railgunit.com.OtherTests
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
        Product product;
        //arrange

        [SetUp]
        public void given_all_revel_categories()
        {
            db = new GrindContext();
            var cats = db.ProductCategories.ToList();
            sut = new ProductCategoryService(cats);
            allCatss = db.ProductCategories.ToList();
        }

        //arrange
        public void when_given_a_product()
        {
            product = db.Products.FirstOrDefault(x => x.resource_uri == "/resources/Product/28/");
        }

        [Test]
        public void should_return_expected_parent_categories()
        {

            var result = sut.GetParentCategory(product);
            result.ShouldNotBeNull();

        }

    }
}
