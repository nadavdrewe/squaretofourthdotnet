using System;
using System.Collections.Generic;
using System.Linq;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.PaymentSummaries
{
    public class PaymentSummaryGroup
    {
        public PaymentSummaryGroup()
        {
            PaymentSummaries = new List<PaymentSummaryForPeriod>();
            PaymentSummaryGroupRequestType = PaymentSummaryGroupRequestType.NullObject;
            TotalAmount = 0.00M;
            TotalTaxAmount = 0.00M;

        }

        public List<PaymentSummaryForPeriod> PaymentSummaries { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }

        public PaymentSummaryGroupRequestType PaymentSummaryGroupRequestType { get; set; }

    }


    interface IGetPaymentSummariesForPeriod
    {
        /// <summary>
        /// Pass in the group, date and establishment you want and it will add the PaymentSummaryForPeriods to the PaymentSummaryGroup 
        /// </summary>
        /// <param name="currentDate"></param>
        /// <param name="nextDate"></param>
        /// <param name="db"></param>
        /// <param name="paymentSummaryGroup"></param>
        /// <param name="establishments"></param>
        /// <returns></returns>
        PaymentSummaryGroup GetPaymentSummariesForPeriod(DateTime currentDate, DateTime nextDate,
            GrindContext db, PaymentSummaryGroup paymentSummaryGroup, List<Establishment> establishments = null);

    }

    public class GetPaymentSummariesForPeriodEstablishmentsInternalService : IGetPaymentSummariesForPeriod
    {
        public PaymentSummaryGroup GetPaymentSummariesForPeriod(DateTime currentDate, DateTime nextDate, GrindContext db,
            PaymentSummaryGroup paymentSummaryGroup, List<Establishment> establishments = null)
        {
            foreach (var establishment in establishments)
            {


                decimal? amount = 0.00M;
                decimal? tax = 0.00M;

                var paymentQueryRecords = db.Payments.Where(
                    x => x.created_date >= currentDate && x.created_date <= nextDate)
                    .Where(x => x.establishment_id == establishment.establishment_id).ToList();

                var taxQueryAmount = db.Orders.Where(
                    x => x.created_date >= currentDate && x.created_date <= nextDate)
                    .Where(x => x.establishment_id == establishment.establishment_id).ToList();


                if (paymentQueryRecords.Count > 0)
                {
                    amount = paymentQueryRecords.Sum(x => x.amount);
                }

                if (taxQueryAmount.Count > 0)
                {
                    tax = taxQueryAmount.Sum(x => x.tax);
                }

                paymentSummaryGroup.PaymentSummaries.Add(new PaymentSummaryForPeriod
                {
                    StartDate = currentDate,
                    EndDate = nextDate,
                    Amount = (decimal)amount,
                    TaxAmount = (decimal)tax,
                    Establishment = establishment.name
                });
            }

            return paymentSummaryGroup;
        }
    }

    public class GetPaymentSummariesForPeriodNoEstablishmensInternalService : IGetPaymentSummariesForPeriod
    {
        public PaymentSummaryGroup GetPaymentSummariesForPeriod(DateTime currentDate, DateTime nextDate, GrindContext db,
            PaymentSummaryGroup paymentSummaryGroup, List<Establishment> establishments = null)
        {
            decimal? amount = 0.00M;
            decimal? tax = 0.00M;

            var paymentQueryRecords = db.Payments.Where(x => x.created_date >= currentDate && x.created_date <= nextDate).ToList()
             ;

            var taxQueryRecords = db.Orders.Where(
                x => x.created_date >= currentDate && x.created_date <= nextDate).ToList();
                           
            if (paymentQueryRecords.Count > 0)
            {
                amount = paymentQueryRecords.Sum(x => x.amount);
            }

            if (taxQueryRecords.Count > 0)
            {
                tax = taxQueryRecords.Sum(x => x.tax);
            }


            paymentSummaryGroup.PaymentSummaries.Add(new PaymentSummaryForPeriod
            {
                StartDate = currentDate,
                EndDate = nextDate,
                Amount = (decimal)amount,
                TaxAmount = (decimal)tax,
                Establishment = "All"
            });

            return paymentSummaryGroup;

        }
    }

    /// <summary>
    /// Factory creates to NEAREST WHOLE DATASET depending on dates entered and type requested..
    /// If you put in 01/01/2015 - 03/01/2015 and chose 'week' you would get 01/08
    /// </summary>
    public class PaymentSummaryGroupFactory
    {
        private GrindContext _db;

        public PaymentSummaryGroupFactory(GrindContext db)
        {
            _db = db;
        }

        public PaymentSummaryGroupFactory()
        {
            _db = new GrindContext();
        }

        public PaymentSummaryGroup Create(DateTime start, DateTime end, PaymentSummaryGroupRequestType requestType,
            List<int> establishmentsIds = null)
        {
            var payments = new List<Payment>();
            var orders = new List<Order>();

            var groupToReturn = new PaymentSummaryGroup();

            //implement strategy via interface and return whole object   
            var strategy = PaymentGroupPeriodStrategyFactory.Create(requestType);
            var instantiatedObject = strategy.Create(start, end, _db, establishmentsIds);

            //set group totals
            instantiatedObject.Start = start;
            instantiatedObject.End = end;
            instantiatedObject.TotalAmount = instantiatedObject.PaymentSummaries.Sum(x => x.Amount);
            instantiatedObject.TotalTaxAmount = instantiatedObject.PaymentSummaries.Sum(x => x.TaxAmount);

            return instantiatedObject;

        }
    }

    public interface IPaymentGroupPeriodStrategy
    {
        PaymentSummaryGroup Create(DateTime start, DateTime end, GrindContext db,
            IEnumerable<int> establishmentIds = null);
    }

    public class NullObjectPaymentGroupStrategy : IPaymentGroupPeriodStrategy
    {
        public PaymentSummaryGroup Create(DateTime start, DateTime end, GrindContext db, IEnumerable<int> establishmentIds = null)
        {
            throw new NotImplementedException();
        }
    }


    public class DayPaymentGroupStrategy : IPaymentGroupPeriodStrategy
    {
        public PaymentSummaryGroup Create(DateTime start, DateTime end, GrindContext db,
            IEnumerable<int> establishmentIds = null)
        {
            List<Payment> payments = new List<Payment>();
            List<Order> orders = new List<Order>();
            List<Establishment> establishments = new List<Establishment>();


            var paymentSummaryGroup = new PaymentSummaryGroup();
            var dateRange = new List<DateTime>();


            paymentSummaryGroup.PaymentSummaryGroupRequestType = PaymentSummaryGroupRequestType.Days;

            var currentDate = start;
            if (establishmentIds != null)
            {
                foreach (var estId in establishmentIds)
                {
                    establishments.Add(db.Establishments.First(x => x.establishment_id == estId));
                }
            }


            while (currentDate < end)
            {
                var nextDate = currentDate.AddDays(1);

                if (establishments.Any())
                {
                    var paymentPeriodService = new GetPaymentSummariesForPeriodEstablishmentsInternalService();
                    paymentPeriodService.GetPaymentSummariesForPeriod(currentDate, nextDate, db, paymentSummaryGroup, establishments);

                }
                else
                {

                    var paymentPeriodService = new GetPaymentSummariesForPeriodNoEstablishmensInternalService();
                    paymentPeriodService.GetPaymentSummariesForPeriod(currentDate, nextDate, db, paymentSummaryGroup, establishments);
                }

                currentDate = currentDate.AddDays(1);
            }


            return paymentSummaryGroup;
        }
    }

    public class WeekPaymentGroupStrategy : IPaymentGroupPeriodStrategy
    {
        public PaymentSummaryGroup Create(DateTime start, DateTime end, GrindContext db, IEnumerable<int> establishmentIds = null)
        {
            List<Payment> payments = new List<Payment>();
            List<Order> orders = new List<Order>();
            List<Establishment> establishments = new List<Establishment>();


            var paymentSummaryGroup = new PaymentSummaryGroup();
            var dateRange = new List<DateTime>();


            paymentSummaryGroup.PaymentSummaryGroupRequestType = PaymentSummaryGroupRequestType.Weeks;

            var currentDate = start;
            if (establishmentIds != null)
            {
                foreach (var estId in establishmentIds)
                {
                    establishments.Add(db.Establishments.First(x => x.establishment_id == estId));
                }
            }


            while (currentDate < end)
            {
                var nextDate = currentDate.AddDays(7);

                if (establishments.Any())
                {
                    var paymentPeriodService = new GetPaymentSummariesForPeriodEstablishmentsInternalService();
                    paymentPeriodService.GetPaymentSummariesForPeriod(currentDate, nextDate, db, paymentSummaryGroup, establishments);

                }
                else
                {

                    var paymentPeriodService = new GetPaymentSummariesForPeriodNoEstablishmensInternalService();
                    paymentPeriodService.GetPaymentSummariesForPeriod(currentDate, nextDate, db, paymentSummaryGroup, establishments);
                }

                currentDate = currentDate.AddDays(7);
            }


            return paymentSummaryGroup;
        }
    }
    
    public class MonthPaymentGroupStrategy : IPaymentGroupPeriodStrategy
    {
        public PaymentSummaryGroup Create(DateTime start, DateTime end, GrindContext db, IEnumerable<int> establishmentIds = null)
        {
            List<Payment> payments = new List<Payment>();
            List<Order> orders = new List<Order>();
            List<Establishment> establishments = new List<Establishment>();


            var paymentSummaryGroup = new PaymentSummaryGroup();
            var dateRange = new List<DateTime>();


            paymentSummaryGroup.PaymentSummaryGroupRequestType = PaymentSummaryGroupRequestType.Months;

            var currentDate = start;
            if (establishmentIds != null)
            {
                foreach (var estId in establishmentIds)
                {
                    establishments.Add(db.Establishments.First(x => x.establishment_id == estId));
                }
            }


            while (currentDate < end)
            {
                var nextDate = currentDate.AddMonths(1);

                if (establishments.Any())
                {
                    var paymentPeriodService = new GetPaymentSummariesForPeriodEstablishmentsInternalService();
                    paymentPeriodService.GetPaymentSummariesForPeriod(currentDate, nextDate, db, paymentSummaryGroup, establishments);

                }
                else
                {

                    var paymentPeriodService = new GetPaymentSummariesForPeriodNoEstablishmensInternalService();
                    paymentPeriodService.GetPaymentSummariesForPeriod(currentDate, nextDate, db, paymentSummaryGroup, establishments);
                }

                currentDate = currentDate.AddMonths(1);
            }


            return paymentSummaryGroup;
        
        }
    }


    public class HourPaymentGroupStrategy : IPaymentGroupPeriodStrategy
    {
        public PaymentSummaryGroup Create(DateTime start, DateTime end, GrindContext db, IEnumerable<int> establishmentIds = null)
        {
            List<Payment> payments = new List<Payment>();
            List<Order> orders = new List<Order>();
            List<Establishment> establishments = new List<Establishment>();


            var paymentSummaryGroup = new PaymentSummaryGroup();
            var dateRange = new List<DateTime>();


            paymentSummaryGroup.PaymentSummaryGroupRequestType = PaymentSummaryGroupRequestType.Hours;

            var currentDate = start;
            if (establishmentIds != null)
            {
                foreach (var estId in establishmentIds)
                {
                    establishments.Add(db.Establishments.First(x => x.establishment_id == estId));
                }
            }


            while (currentDate < end)
            {
                var nextDate = currentDate.AddHours(1);

                if (establishments.Any())
                {
                    var paymentPeriodService = new GetPaymentSummariesForPeriodEstablishmentsInternalService();
                    paymentPeriodService.GetPaymentSummariesForPeriod(currentDate, nextDate, db, paymentSummaryGroup, establishments);

                }
                else
                {

                    var paymentPeriodService = new GetPaymentSummariesForPeriodNoEstablishmensInternalService();
                    paymentPeriodService.GetPaymentSummariesForPeriod(currentDate, nextDate, db, paymentSummaryGroup, establishments);
                }

                currentDate = currentDate.AddHours(1);
            }


            return paymentSummaryGroup;
        }
    }
    

    /* public class WeekPaymentGroupStrategy : IPaymentGroupPeriodStrategy
     {
         public PaymentSummaryGroup Create(List<Payment> payment)
         {

         }
     }

     public class MonthPaymentGroupStrategy : IPaymentGroupPeriodStrategy
     {

         public PaymentSummaryGroup Create(IEnumerable<Payment> payment)
         {

         }
     }*/



    public static class PaymentGroupPeriodStrategyFactory
    {
        public static IPaymentGroupPeriodStrategy Create(PaymentSummaryGroupRequestType type)
        {
            IPaymentGroupPeriodStrategy paymentGroupStrategy = new NullObjectPaymentGroupStrategy();

            switch (type)
            {
                case PaymentSummaryGroupRequestType.Hours:
                    paymentGroupStrategy = new HourPaymentGroupStrategy();
                    break;
                case PaymentSummaryGroupRequestType.Days:
                    paymentGroupStrategy = new DayPaymentGroupStrategy();
                    break;
                case PaymentSummaryGroupRequestType.Weeks:
                    paymentGroupStrategy = new WeekPaymentGroupStrategy();
                    break;
                case PaymentSummaryGroupRequestType.Months:
                    paymentGroupStrategy = new MonthPaymentGroupStrategy();
                    break;
                default:
                    throw new Exception();
            }

            return paymentGroupStrategy;
        }
    }

    public enum PaymentSummaryGroupRequestType
    {
        Hours, Days, Weeks, Months, NullObject
    }
}
