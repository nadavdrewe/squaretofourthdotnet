using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using api.grind._808nd.com.Controllers;
using NUnit.Core;
using NUnit.Framework;
using Revel._808nd.com.Classes.PaymentSummaries;
using Revel._808nd.com.Models;
using Should;

namespace apitests.grind._808nd.com.IntegrationTests.Controller
{
    [TestFixture]
    public class PaymentControllerSpecs
    {
        private static PaymentsController SUT;

        [TestFixture]
        public class GetSpecs
        {
            static decimal amountResult;
            static decimal taxResult;
            static DateTime start = new DateTime(2015, 02, 01, 02, 00, 00);
            private static int timeToAdd = 2;
            static  IHttpActionResult result;
            static GrindContext db = new GrindContext();

            [TestFixtureSetUp]
            public void Given()
            {
                SUT = new PaymentsController();
            }

            [TestFixture]
            public class AllEstablishments
            {
               
              

                [Test]
                public async Task then_result_is_OkNegotiatedContentResult_of_correct_type_and_number_of_summaries_is_correct()
                {
                    result = await SUT.GetPayments(start, timeToAdd, PaymentSummaryGroupRequestType.Days) as OkNegotiatedContentResult<PaymentSummaryGroup>;

                    result.ShouldBeType<OkNegotiatedContentResult<PaymentSummaryGroup>>();
                    ((OkNegotiatedContentResult < PaymentSummaryGroup >)result).Content.PaymentSummaryGroupRequestType.ShouldEqual(PaymentSummaryGroupRequestType.Days);
                    ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content.PaymentSummaries.Count().ShouldEqual(2);
                }

                [Test]
                public async Task then_a_months_worth_of_content_pulls_4_results()
                {
                     DateTime monthStart = new DateTime(2015, 02, 01, 02, 00, 00);
                    int weekstimeToAdd = 4;


                    result = await SUT.GetPayments(monthStart, weekstimeToAdd, PaymentSummaryGroupRequestType.Weeks) as OkNegotiatedContentResult<PaymentSummaryGroup>;
                    ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content.PaymentSummaryGroupRequestType.ShouldEqual(PaymentSummaryGroupRequestType.Weeks);
                    ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content.PaymentSummaries.Count().ShouldEqual(4);


                }


                [Test]
                public async Task then_2_weeks_in_week_mode_pulls_two_results_that_matches_the_db()
                {
                
                    result = await SUT.GetPayments(start, timeToAdd, PaymentSummaryGroupRequestType.Weeks) as OkNegotiatedContentResult<PaymentSummaryGroup>;
                    var content =
                        ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content;

                    //EVEN THOUGH PUT IN 2 DAYS, BECAUSE IT'S IN 
                    var expectedAmount = db.Payments.Where(
                    x => x.created_date >= content.Start && x.created_date <= content.End)
                  .Sum(x => x.amount);

                    var expectedTax = db.Orders.Where(
                        x => x.created_date >= content.Start && x.created_date <= content.End)
                     
                        .Sum(x => x.tax);

                    ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content.TotalAmount.ShouldEqual(expectedAmount);
                    ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content.TotalTaxAmount.ShouldEqual(expectedTax);
                    ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content.PaymentSummaries.Count().ShouldEqual(2);
                }


                [Test]
                public async Task then_In_month_mode_3_months_returns_3_records_and_results_match_the_db()
                {
                    DateTime monthStart = new DateTime(2015, 02, 01, 02, 00, 00);
                    int monthsToAdd = 3;


                    result = await SUT.GetPayments(monthStart, monthsToAdd, PaymentSummaryGroupRequestType.Months) as OkNegotiatedContentResult<PaymentSummaryGroup>;
                    ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content.PaymentSummaryGroupRequestType.ShouldEqual(PaymentSummaryGroupRequestType.Months);
                    ((OkNegotiatedContentResult<PaymentSummaryGroup>)result).Content.PaymentSummaries.Count().ShouldEqual(3);
                }
                               
            }

            [TestFixture]
            public class MultipleEstablishments
            {
                              

                
            }
        }
    }
}
