using domain.pipeline.fourth.com.SalesFactories;
using domain.pipeline.fourth.com.SalesFactories.Helper;
using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using NUnit.Framework;
using com.fourth.pipeline.pos.Extensions;
using com.fourth.pipeline.pos.Model;
using Shouldly;
using square.pipeline.fourth.com.Extensions;
using square.pipeline.fourth.com.Services;
using Square;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests.domain.pipelines.fourth.com
{
    [TestFixture]
    public class ActivityCodeServiceTests
    {

        RecordActivityCodeService SUT;

        [SetUp]
        public async Task SetUp()
        {

            SUT = new RecordActivityCodeService();
        }

        [Test]
        public void Check_Service_Increments_Correctly_Then_Resets()
        {

            for (int i = 0; i < 10; i++)
            {
                SUT.GetCurrentActivityCode().ShouldBe(i.ToString());
                SUT.Increment();
            }

            SUT.ResetToZero();
            SUT.GetCurrentActivityCode().ShouldBe("0");
        }
    }
}
