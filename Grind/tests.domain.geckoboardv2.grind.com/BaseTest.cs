using domain.geckoboardv2.grind.com.Services;
using NUnit.Framework;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests.domain.geckoboardv2.grind.com
{
    [TestFixture]
    public class BaseTest
    {

        GrindContext grindDb;
     

        [SetUp]
        public void SetUp()
        {
            grindDb = new GrindContext();           
        }
    }
}
