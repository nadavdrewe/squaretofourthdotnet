using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Reporting.Caternet;
using Revel._808nd.com.Classes.ServiceImplemenations;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Grind;

namespace tests.mapping.xero.railgunit.com.XeroMappingTests
{

    [TestFixture]
    public class ProductClassSummaryBucketTests
    {
        List<CaternetItemSummary> data;
        ProductClassService prodClassService;
        List<Product> products;
        List<ProductClass> productClasses;
        RevelClassTaxMappingService SUT;


        /// <summary>
        /// Using London Grind live data
        /// </summary>
        [SetUp]
        public void Arrange()
        {
            GrindContext db = new GrindContext();
            DateTime syncStart = new DateTime(2017, 12, 01, 03, 00, 00);
            DateTime syncEnd = new DateTime(2017, 12, 07, 03, 00, 00);
            SUT = new RevelClassTaxMappingService();


            prodClassService = new ProductClassService("rsedfsdfwe", "http://test.com", db);
            string est = "4";

            //get summed items as per RevelUp
            var startParam = new SqlParameter("@startDate", SqlDbType.DateTime);
            startParam.Value = syncStart;
            var endParam = new SqlParameter("@endDate", SqlDbType.DateTime);
            endParam.Value = syncEnd;
            var estParam = new SqlParameter("@establishmentId", SqlDbType.Int);
            estParam.Value = est;

            //PROC FILTERS OUT VOIDS - WE NEED TO KEEP IN VOID / COMP AMOUNT BUT NOT PURESALES
            //DELETED ARE REMOVED
            try
            {
                products = db.Products.Where(x => x.establishment_id == 4).ToList();
                data = db.Database.SqlQuery<CaternetItemSummary>(
                      "Revel_GenerateCaternetSummary @startDate, @endDate, @establishmentId", startParam, endParam, estParam).ToList();

            }
            catch (Exception ex)
            {

                throw;
            }
            productClasses = db.ProductClasses.ToList();

        }

        public void Should_Generate_Some_Buckets()
        {
      
           



        }
    }

}
