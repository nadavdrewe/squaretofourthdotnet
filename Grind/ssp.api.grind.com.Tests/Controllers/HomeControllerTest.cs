using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ssp.api.grind.com;
using ssp.api.grind.com.Controllers;

namespace ssp.api.grind.com.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
