using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;


namespace tests.fourth.revel.com
{
    [TestFixture]
    public class RevelDbReaderTests
    {

        [Test]
        public void ShouldReturnSomething()
        {
            //arr
            var sut = new Mock<IRevelReader>(MockBehavior.Strict);
            var product = new Product
          {
              active = "true",
              resource_uri = "testURI",
              name = "Test Product"
          };

            var prodList = new List<Product>();
            prodList.Add(product);


            sut.Setup(x => x.GetProducts(It.IsAny<int>())).Returns(() => prodList
                );

            sut.Setup(x => x.GetProducts(It.IsInRange(123, 12345, Range.Exclusive)))
                .Throws<Exception>();

            //act
            var result = sut.Object.GetProducts(123);


            //assert
            Assert.That(result, Is.TypeOf<List<Product>>());

            Assert.That(result, Is.Not.Empty);

            Assert.That(result.First().name, Is.StringMatching("Test Product").IgnoreCase);

            //method has been called
            sut.Verify(x => x.GetProducts(It.IsAny<int>()), Times.AtLeastOnce);

            sut.Verify(x => x.GetProducts(It.Is<int>(y => y.Equals(123))));
        }

        [Test]
        public void ShouldHaveProperty_NameSetToGobbler()
        {
            //arrange
            var mockCOntext = new Mock<RevelContextBase>("test");
            mockCOntext.SetupAllProperties();
      
            var sut = new CustomerService(mockCOntext.Object);

            //act

            var result = mockCOntext.Object.Brands.ToList();

            Assert.That(result, Is.InstanceOf<List<Brand>>());

        }

    }
}
