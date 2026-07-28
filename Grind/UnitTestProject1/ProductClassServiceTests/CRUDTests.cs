using NUnit.Framework;
using Revel._808nd.com.Classes.ServiceImplemenations;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace UnitTestProject1.ProductClassServiceTests
{
    [TestFixture]
    public class CRUDTests
    {
        ProductClassService SUT;
        GrindContext db;
        string RevelBaseURL = "https://shoreditchgrind.revelup.com/";
        string RevelAPIKEY = "408d6c05f2864ece90c037333d64f333:9ae943831e7f443b9edf3a6203e66598290fc7d2f3244ca9b69dd67404aa39f2";


        [SetUp]
        public void given_a_request_to_update_classes()
        {
            db = new GrindContext("GrindLiveContext");
            SUT = new ProductClassService(RevelAPIKEY, RevelBaseURL, db);
        }

        //arrange
        public void when_()
        {

        }

        [Test]
        public async Task should_get_classes_from_revel_and_update_local()
        {
            try
            {
                var existing = db.ProductClasses.ToList();
                await SUT.GetAllProductClassesAndReplaceLocal();
                var result = db.ProductClasses.ToList();
                result.ShouldNotBeEmpty();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
