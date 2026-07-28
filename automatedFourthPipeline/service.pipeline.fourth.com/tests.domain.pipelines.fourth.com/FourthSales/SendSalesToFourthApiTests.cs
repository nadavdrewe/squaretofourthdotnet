using com.fourth.pipeline.pos.Services.SalesApi;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    [Explicit("Requires live Fourth stage credentials and endpoint availability.")]
    public class SendSalesToFourthApiTests
    {
        string STAGEUserName = "Blue_bird";
        string STAGEuserLogin = "H<2CvmVr%m}+FqL&";
        string STAGEurl = "https://api-dev.fourth.com/prelive/api/eposgateway";
        string STAGEtokenURL = "https://api-dev.fourth.com/prelive/api/eposgateway/Token";
        string payload = "username={0}&password={1}&grant_type=password";
        FourthApiService SUT;

        public async Task Arrange()
        {
            SUT = new FourthApiService(STAGEUserName, STAGEuserLogin, STAGEurl);
            await SUT.Login();
        }

        [SetUp]
        public async Task Act()
        {
            await Arrange();
        }

        [Test]
        public async Task GenerateCSVAndSendToFourth()
        {

        }

    }
}
