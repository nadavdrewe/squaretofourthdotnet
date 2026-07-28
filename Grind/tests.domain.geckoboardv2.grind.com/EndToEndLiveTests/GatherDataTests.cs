using domain.geckoboardv2.grind.com.Services;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests.domain.geckoboardv2.grind.com.EndToEndLiveTests
{
    [TestFixture]
    public class GatherDataTests 
    {
        GeckoboardV2Service SUT;

        [SetUp]
        public void SetUp()
        {
            SUT = new GeckoboardV2Service();
            //base.SetUp();
        }

        [Test]
        public async Task GetDataForToday()
        {
            var todayNow = DateTime.Now;
            var actualStart = new DateTime(todayNow.Year, todayNow.Month, todayNow.Day, 03, 00, 00);
            //get the data!!!
            await SUT.Bootstrap();
            await SUT.GatherAllDailyAndComparisonRawData(todayNow);

            SUT.peristenceDataWrappersToday.Count().ShouldBeGreaterThan(5);
            SUT.peristenceDataWrappersToday.Count().ShouldBeGreaterThan(5);
        }
    }
}
