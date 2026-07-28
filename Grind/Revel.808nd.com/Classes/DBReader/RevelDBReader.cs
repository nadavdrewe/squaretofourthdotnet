using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Models;
using RI = Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public class RevelDBReader : RI.IRevelReaderAsync
    {

        public Establishment Establishment { get; set; }

        public RevelDBReader(Establishment establishment)
        {
            this.Establishment = establishment;
        }

        Task<List<OrderItem>> RI.IRevelReaderAsync.GetOrderItems(DateTime StartDate, DateTime EndDate)
        {
            return GetOrderItems(StartDate, EndDate);
        }



        public virtual async Task<List<Payment>> GetPayments(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                using (GrindContext _db = new GrindContext())
                {
                    _db.Database.CommandTimeout = 480;
                    var pay = await _db.Payments
                        .Where(x => x.created_date >= StartDate)
                      .Where(x => x.created_date <= EndDate).
                        ToListAsync();

                    return pay;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }


        public async Task<List<Payment>> GetPaymentsFixedStartTime(DateTime StartDate, DateTime EndDate)
        {
            var fixedStartTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 02, 00, 00);

            try
            {
                using (GrindContext _db = new GrindContext())
                {
                    var pay = await _db.Payments.AsNoTracking()
                        .Where(x => x.created_date >= fixedStartTime)
                      .Where(x => x.created_date <= EndDate)
                      .AsNoTracking()
                        .ToListAsync();

                    return pay;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }


        public async Task<List<ProductCategory>> GetProductCategories()
        {

            try
            {
                using (GrindContext _db = new GrindContext())
                {

                    var var2 = await _db.ProductCategories.AsNoTracking().ToListAsync();

                    return var2;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual async Task<List<Product>> GetProducts()
        {
            try
            {
                using (GrindContext _db = new GrindContext())
                {
                    var prods = await _db.Products.AsNoTracking().ToListAsync();
                    return prods;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public async Task<List<Product>> GetProductsNoEstablishment()
        {
            try
            {
                using (GrindContext _db = new GrindContext())
                {
                    var prods = await _db.Products.AsNoTracking().ToListAsync();
                    return prods;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<List<ProductCategory>> GetProductCategoriesNoEstablishment()
        {
            try
            {
                using (GrindContext _db = new GrindContext())
                {
                    var prodCats = await _db.ProductCategories.AsNoTracking().ToListAsync();
                    return prodCats;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<OrderItem>> GetOrderItems(DateTime StartDate, DateTime EndDate)
        {

            using (GrindContext _db = new GrindContext())
            {

                //need to pull the criteria used in the RevelUpBackend
                List<OrderItem> items = await (_db.OrderItems.AsNoTracking()
                    .Where(x => x.created_date >= StartDate)
                    .Where(x => x.created_date <= EndDate)
                    /*.Where(x=>x.voided_reason = "")
                    .Where(x=>x.voided_reason == null)*/
                    ).ToListAsync();


                return items;
            }

        }

        public async Task<List<OrderItem>> GetOrderItems(int OrderID)
        {
            using (GrindContext _db = new GrindContext())
            {
                _db.Database.CommandTimeout = 480;
                List<OrderItem> items = await (_db.OrderItems.AsNoTracking()
                    .Where(x => x.parent_order_id.Equals(OrderID))
                    .Where(x => x.voided_reason == "")
                    .Where(x => x.voided_reason == null)
                    ).ToListAsync();


                return items;
            }


        }

        public async Task<List<Order>> GetOrders(Establishment establishment,
            DateTime StartDate, DateTime EndDate)
        {

            using (GrindContext _db = new GrindContext())
            {
                List<Order> items = await (_db.Orders.AsNoTracking()
                    .Where(x => x.establishment.Equals(establishment))
                    .Where(x => x.created_date >= StartDate)
                    .Where(x => x.created_date <= EndDate)
                    //.Where(x => x.closed.Equals(true))
                    ).ToListAsync();


                return items;
            }

        }

        public async Task<List<Order>> GetOrdersSinglePull(DateTime StartDate, DateTime EndDate)
        {
            var itemsToReturn = new List<Order>();

            using (GrindContext _db = new GrindContext())
            {

                var maxDateTimeOrder = EndDate;
                var currentMinimum = StartDate;


                while (currentMinimum < EndDate)
                {

                    var itemsReturned = _db.Orders.AsNoTracking()
                        .Where(x => x.created_date > currentMinimum && x.created_date <= EndDate).OrderBy(x => x.created_date).Take(20000).ToList();

                    if (itemsReturned.Count().Equals(0))
                    {
                        break;
                    }

                    itemsToReturn.AddRange(itemsReturned);
                    currentMinimum = (DateTime)itemsReturned.Max(x => x.created_date);

                }


                return itemsToReturn;

            }


        }

        public async Task<List<Discount>> GetDiscounts()
        {

            try
            {
                using (GrindContext _db = new GrindContext())
                {

                    var var2 = await _db.Discounts.AsNoTracking().ToListAsync();

                    return var2;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// Makes multiple calls to the context
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetOrdersBulk(RevelContextBase _db, DateTime startDate, DateTime endDate)
        {
            var itemsToReturn = new List<Order>();

            var maxDateTimeOrder = endDate;
            var currentMinimum = startDate;


            while (currentMinimum < endDate)
            {

                var itemsReturned = _db.Orders.AsNoTracking().OrderBy(x => x.created_date)
                    .Where(x => x.created_date > currentMinimum && x.created_date <= endDate).Take(200000).ToList();

                if (itemsReturned.Count().Equals(0))
                {
                    break;
                }

                itemsToReturn.AddRange(itemsReturned);
                currentMinimum = (DateTime)itemsReturned.Max(x => x.created_date);

            }


            return itemsToReturn;
        }

        public async Task<List<T>> GetRevelType<T>() where T : class
        {
            try
            {

                using (GrindContext _db = new GrindContext())
                {
                    var data = await _db.Set<T>().ToListAsync();
                    return data;

                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error in GetRevelType of DBReader repository", ex);
            }

        }

        public async Task<List<T>> GetRevelTypeNoTracking<T>() where T : class
        {
            try
            {

                using (GrindContext _db = new GrindContext())
                {
                    var data = await _db.Set<T>().AsNoTracking().ToListAsync();
                    return data;

                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error in GetRevelType of DBReader repository", ex);
            }

        }


    }
}
