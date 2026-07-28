using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Effort.DataLoaders;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.PaymentSummaries;
using Revel._808nd.com.Models;
using Should;

namespace apitests.grind._808nd.com.UnitTests
{
    [TestFixture]
    public class PaymentSummaryGroupTests
    {
        [TestFixture]
        public class PaymentSummaryGroupNewObjectTests
        {
            private PaymentSummaryGroup SUT;
            //arrange
            [SetUp]
            public void Setup_Tests()
            {
                SUT = new PaymentSummaryGroup();
            }

            [Test]
            public void new_payment_summary_group_is_of_null_object_type()
            {
                SUT.PaymentSummaryGroupRequestType.ShouldEqual(PaymentSummaryGroupRequestType.NullObject);
                SUT.PaymentSummaryGroupRequestType.ShouldNotEqual(PaymentSummaryGroupRequestType.Days);
            }

            [Test]
            public void new_payment_summary_group_has_zero_payment_summaries()
            {
                SUT.PaymentSummaries.Count().ShouldBeLessThan(1);
            }

        }




        [TestFixture]
        public class PaymentSummaryForPeriodTests
        {
            private PaymentSummaryForPeriod SUT;

            [SetUp]
            public void SetUp()
            {
                SUT = new PaymentSummaryForPeriod();

            }

        }

        [TestFixture]
        public class PaymentSummaryGroupFactoryTests
        {
            private PaymentSummaryGroupFactory SUT;
            private EntityDataLoader dataLoader;
            private DbConnection connection;
            private GrindContext db;
            private IEnumerable<Payment> result;
            private PaymentSummaryGroup hourGroupResult;
            private PaymentSummaryGroup dayGroupResult;
            private PaymentSummaryGroup weekGroupResult;
            private PaymentSummaryGroup monthGroupResult;
            private PaymentSummaryGroup zeroGroupResult;

            private decimal dayGroupResultExpectedAmount;
            private decimal dayGroupResultExpectedTaxAmount;

            //arrange
            [SetUp]
            public void Given()
            {
                
                db = new GrindContext();

                SUT = new PaymentSummaryGroupFactory(db);

                When();
            }

            public void When()
            {
                var timespantoAddint = 2;
                var start = new DateTime(2015, 02, 01, 02, 00, 00);
                var end = start.AddDays(2);

                int establihsment = 1;
                var establishments = new List<int> { establihsment };

                result = db.Payments.Take(100).ToList();

                dayGroupResult = SUT.Create(start,
                    end,
                    PaymentSummaryGroupRequestType.Days,
                    establishments
                    );

                dayGroupResultExpectedAmount = db.Payments.Where(
                    x => x.created_date >= start && x.created_date <= end)
                    .Where(x => x.establishment_id == establihsment)
                    .Sum(x => x.amount);

                dayGroupResultExpectedTaxAmount = db.Orders.Where(
                    x => x.created_date >= start && x.created_date <= end)
                    .Where(x => x.establishment_id == establihsment)
                    .Sum(x => x.tax);


            }

            [Test]
            public void then_there_are_some_records_in_the_db()
            {
                result.Count().ShouldBeGreaterThan(1);
                result.Count().ShouldBeLessThan(101);
                result.ShouldImplement<IList<Payment>>();
            }

            [Test]
            public void then_all_day_group_result_is_created()
            {
                dayGroupResult.PaymentSummaries.Count().ShouldBeGreaterThan(0);
                dayGroupResult.PaymentSummaryGroupRequestType.ShouldEqual(PaymentSummaryGroupRequestType.Days);
            }

            [Test]
            public void then_results_match_expected_results()
            {
                dayGroupResult.PaymentSummaries.Sum(x => x.Amount).ShouldEqual(dayGroupResultExpectedAmount);
                dayGroupResult.PaymentSummaries.Sum(x => x.TaxAmount).ShouldEqual(dayGroupResultExpectedTaxAmount);
            }

            [Test]
            public void when_pulling_back_zero_result_set_it_returns_zero_and_doesnt_error_all_establishments()
            {

                zeroGroupResult = SUT.Create(new DateTime(2012, 01, 01),
                    new DateTime(2012, 01, 02),
                    PaymentSummaryGroupRequestType.Days
                    );

                zeroGroupResult.TotalAmount.ShouldEqual(0.00M);
                zeroGroupResult.TotalTaxAmount.ShouldEqual(0.00M);
            }

            [Test]
            public void when_pulling_back_zero_result_set_it_returns_zero_and_doesnt_error_single_establishments()
            {

                zeroGroupResult = SUT.Create(new DateTime(2012, 01, 01),
                    new DateTime(2012, 01, 02),
                    PaymentSummaryGroupRequestType.Days,
                    new List<int> { 1 }
                    );

                zeroGroupResult.TotalAmount.ShouldEqual(0.00M);
                zeroGroupResult.TotalTaxAmount.ShouldEqual(0.00M);
            }

            [Test]
            public void when_passing_in_zero_establishments_results_are_correct()
            {
                DateTime startDay = new DateTime(2015, 02, 01);
                DateTime endDay = new DateTime(2015, 02, 03);
                ;

                dayGroupResult = SUT.Create(startDay, endDay, PaymentSummaryGroupRequestType.Days);

                dayGroupResultExpectedAmount = db.Payments.Where(
                    x => x.created_date >= startDay && x.created_date <= endDay)
                    .Sum(x => x.amount);

                dayGroupResultExpectedTaxAmount = db.Orders.Where(
                    x => x.created_date >= startDay && x.created_date <= endDay)
                    .Sum(x => x.tax);


                dayGroupResult.TotalAmount.ShouldEqual(dayGroupResultExpectedAmount);
                dayGroupResult.TotalTaxAmount.ShouldEqual(dayGroupResultExpectedTaxAmount);
            }

            [Test]
            public void check_number_of_payment_summaries_conforms_to_expected_time_period_for_single_Establishment()
            {
                DateTime startDay = new DateTime(2015, 02, 01);
                DateTime endDay = new DateTime(2015, 02, 03);
                ;

                dayGroupResult = SUT.Create(startDay, endDay, PaymentSummaryGroupRequestType.Days);
                dayGroupResult.PaymentSummaries.Count().ShouldEqual(2);

            }


            [Test]
            public void check_individual_establishment_totals_are_correct_and_number_of_summaries_count_is_as_expected()
            {
                DateTime startDay = new DateTime(2015, 07, 01);
                DateTime endDay = new DateTime(2015, 07, 08);
                ;
                var establishments = new List<int> { 1, 4 };

                dayGroupResult = SUT.Create(startDay, endDay, PaymentSummaryGroupRequestType.Days, establishments);

                var shoreAmount =
                    dayGroupResult.PaymentSummaries.Where(x => x.Establishment == "Shoreditch").Sum(x => x.Amount);
                var shoreTax =
                    dayGroupResult.PaymentSummaries.Where(x => x.Establishment == "Shoreditch").Sum(x => x.TaxAmount);

                var londonAmount =
                    dayGroupResult.PaymentSummaries.Where(x => x.Establishment == "London").Sum(x => x.Amount);
                var londonTax =
                    dayGroupResult.PaymentSummaries.Where(x => x.Establishment == "London").Sum(x => x.TaxAmount);

                var shoreExpectedAmount = db.Payments.Where(
                    x => x.created_date >= startDay && x.created_date <= endDay
                         && x.establishment_id == 1).Sum(x => x.amount);

                var shoreExpectedTax = db.Orders.Where(
                    x => x.created_date >= startDay && x.created_date <= endDay
                         && x.establishment_id == 1)
                    .Sum(x => x.tax);

                var londonExpectedAmount = db.Payments.Where(
                    x => x.created_date >= startDay && x.created_date <= endDay
                         && x.establishment_id == 4).Sum(x => x.amount);

                var londonExpectedTax = db.Orders.Where(
                    x => x.created_date >= startDay && x.created_date <= endDay
                         && x.establishment_id == 4)
                    .Sum(x => x.tax);

                shoreAmount.ShouldEqual(shoreExpectedAmount);
                shoreTax.ShouldEqual(shoreExpectedTax);

                londonExpectedAmount.ShouldEqual(londonExpectedAmount);
                londonExpectedTax.ShouldEqual(londonExpectedTax);

                dayGroupResult.PaymentSummaries.Count().ShouldEqual(14);
            }


            [Test]
            public void number_of_hours_is_as_expected_when_getting_in_hours()
            {
                DateTime startDay = new DateTime(2015, 07, 01);
                DateTime endDay = new DateTime(2015, 07, 02);
                var establishments = new List<int> { 1 };

                //23?? hours of payment??

                hourGroupResult = SUT.Create(startDay, endDay, PaymentSummaryGroupRequestType.Hours, establishments);
                hourGroupResult.PaymentSummaries.Count().ShouldEqual(24);
                

                establishments = new List<int> {1,4,5};
                hourGroupResult = SUT.Create(startDay, endDay, PaymentSummaryGroupRequestType.Hours, establishments);
                hourGroupResult.PaymentSummaries.Count().ShouldEqual(24 * establishments.Count());

            }



        }
    }
}
