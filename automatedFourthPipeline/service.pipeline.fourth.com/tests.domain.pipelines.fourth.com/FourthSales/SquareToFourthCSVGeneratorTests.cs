using domain.pipeline.fourth.com.Services.Square;
using NUnit.Framework;
using com.fourth.pipeline.pos.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    public class SquareToFourthCSVGeneratorTests
    {
        DateTime startUTC;
        DateTime endUTC;
        SquareToFourthCSVGenerator SUT;

        string emailNadzliveApiKey = "";
        string emailNadzToken = "";

        //TEST STATIC VARIOABLES
        string testUnitId = "TEST_UNIT_ID";

        [SetUp]
        public async Task Setup()
        {
            SUT = new SquareToFourthCSVGenerator(emailNadzToken);

            startUTC = new DateTime(2019, 08, 28, 03, 00, 00);
            endUTC = startUTC.AddDays(10);

        }

        [Test]
        public async Task Get_Some_Data()
        {
            //await SUT.GatherData(startUTC, endUTC);
        }

        [Test]
        public async Task Get_Some_Data_Then_Create_The_CSV()
        {
            //await SUT.GatherData(startUTC, endUTC);
            //var data = SUT.CreateSalesRows(testUnitId);

            //data.ShouldNotBeNull<IEnumerable<TransactionDatasetRow>>();
        }

    }
}
