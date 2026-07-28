using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Revel._808nd.com.Classes;


namespace tests.fourth.revel.com
{ 
  
    [Category("cat2")]
    public class Class1
    {

        [SetUp]
        public void BeforeEachTest()
        {
            Debug.Write("Before {0}", TestContext.CurrentContext.Test.Name);


        }

        [TearDown]
        public void AfterEachTest()
        {
            Console.WriteLine("After {0}", TestContext.CurrentContext.Test.Name);


        }

        [Test]
        public void ShouldCreateOrderItem()
        {

            var sut = new object();

            var result = 1 + 2;

            Assert.That(result, Is.EqualTo(2));
        }
        [Test]
        public void ShouldTestStringEquality()
        {

            var sut = "test this string";

            var result = string.Concat(sut, " pug");

            Assert.That(result, Is.EqualTo("test this string pug").IgnoreCase);
        }

        [Test]
        public void ShouldTestStringNonEquality()
        {

            var sut = "test this string";

            var result = string.Concat(sut, " pug");

            Assert.That(result, Is.Not.EqualTo("afdafsde").IgnoreCase);
        }

        [Test]
        public void ShouldTestDoublesValueEquals()
        {

            var sut = 1.1 + 2.2;

            var result = sut;

            Assert.That(result, Is.EqualTo(3.3).Within(1).Percent);
        }

        [Test]
        public void IsNotNotANumber()
        {

            var sut = "test";

            var result = sut;

            Assert.That(result, Is.Not.NaN);
        }

        [Test]
        public void TestShouldBeCorrectDate()
        {
            var sut = "";

            var result = new DateTime(2013, 01, 01);

            Assert.That(result, Is.EqualTo(new DateTime(2014, 01, 01)).Within(TimeSpan.FromDays(366)));

        }

        [Test]
        public void ShouldBeWithinRange()
        {
            var sut = 100;

            var result = sut++;

            Assert.That(result, Is.InRange(sut, 200));

        }

        [Test]
        public void ShouldBeAllNotEmpty()
        {
            var sut = new List<string>();

            sut.Add("first");
            sut.Add("tt");

            Assert.That(sut, Is.All.Not.Empty);

        }

        [Ignore]
        public void ShouldBeAtLeastOneFirst()
        {
            var sut = new List<string>();

            sut.Add("first");
            sut.Add("tt");

            Assert.That(sut, Contains.Item("sword"));

        }

        [Test]
        public void ShouldHaveTwoItemsThatEndWithT()
        {
            var sut = new List<string>();

            sut.Add("first");
            sut.Add("tt");

            Assert.That(sut, Has.Exactly(2).EndsWith("t"));

        }

        [Test]
        public void ShouldHaveSomeEndWithT()
        {
            var sut = new List<string>();

            sut.Add("first");
            sut.Add("tt");

            Assert.That(sut, Has.Some.EndsWith("t"));

        }

        [Test]
        public void ShouldAllBeUnique()
        {
            var sut = new List<string>();

            sut.Add("first");
            sut.Add("tt");

            Assert.That(sut, Is.Unique);

        }

        [Test]
        public void ShouldNotBeAPlonker()
        {
            var sut = new List<string>();

            sut.Add("first");
            sut.Add("tt");

            Assert.That(sut, Has.None.EqualTo("plonker"));

        }


        [Test]
        [Category("cat1")]
        [Repeat(1000)]
        public void ShouldBeResult([Values(0, 0, 0)] int number1, [Values(0, 0, 0)] int number2, [Values(0, 0, 0)]int expectedResult)
        {

            var result = number1 - number2;

            Assert.That(result, Is.EqualTo(expectedResult));

        }

        public class ExampleTestCaseSource : IEnumerable
        {

            public IEnumerator GetEnumerator()
            {
                yield return new[] { 5, 5, 0 };
                yield return new[] { 10, 20, -10 };
                yield return new[] { 10, 20, 10 };
                yield return new[] { 0, 0, 0 };
            }
        }
    }
}
