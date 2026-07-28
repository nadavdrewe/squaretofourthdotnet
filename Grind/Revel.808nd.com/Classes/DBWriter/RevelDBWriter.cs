using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{
    public class RevelDBWriter : IRevelWriter
    {
        public RevelContextBase db { get; set; }

        public RevelDBWriter(RevelContextBase _db)
        {
            db = _db;
        }
        public int SaveDiscounts(List<Discount> discount)
        {

            try
            {
                db.Discounts.AddRange(discount);
                db.SaveChanges();
                return 0;


            }
            catch (Exception x)
            {

                throw x;
            }

        }

        public bool SaveProductCategory(ProductCategory pc)
        {
            try
            {

                db.ProductCategories.Add(pc);
                db.SaveChanges();
                return true;


            }
            catch (Exception x)
            {

                throw x;
            }



        }

        public bool SaveOrderItems(List<OrderItem> theItems)
        {
            try
            {


                db.OrderItems.AddRange(theItems);
                db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public bool SaveProducts(List<Product> prods)
        {

            db.Products.AddRange(prods);
            db.SaveChanges();
            return true;


        }

        public bool SaveOrders(List<Order> orders)
        {
            try
            {

                db.Orders.AddRange(orders);
                db.SaveChanges();
                return true;



            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool SaveProductCategories(List<ProductCategory> pc)
        {
            try
            {

                db.ProductCategories.AddRange(pc);
                db.SaveChanges();
                return true;



            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public bool SavePayments(List<Payment> pay)
        {
            try
            {

                db.Payments.AddRange(pay);
                var ok = db.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool SaveCustomers(List<Customer> payload)
        {
            try
            {

                db.Customers.AddRange(payload);
                db.SaveChanges();

                var addressesToSave = new List<Address>();

                foreach (var customer in payload)
                {
                    if (customer.Addresses != null)
                    {
                        addressesToSave.AddRange(customer.Addresses);
                    }

                }

                if (addressesToSave.Any())
                {
                    db.SaveChanges();
                }


                return true;




            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public bool SaveAddresses(List<Address> payload)
        {
            try
            {

                db.Addresses.AddRange(payload);
                db.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public bool SaveRewardsCardNew(List<RewardsCardNew> payload)
        {
            try
            {

                db.RewardsCardNew.AddRange(payload);
                db.SaveChanges();
                return true;



            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public async Task<int> SaveRevelType<T>(IEnumerable<T> theDataToSave) where T : class
        {
            try
            {


                db.Set<T>().AddRange(theDataToSave);

                if (theDataToSave.Any())
                {
                    var noOfSavedRecords = db.SaveChangesAsync();
                    return noOfSavedRecords.Result;
                }


            }
            catch (Exception ex)
            {

                throw new Exception("There was a problem saving records to the DB using RevelDBWriter", ex);
            }

            return 0;
        }

        public async Task<int> UpdateRevelType<T>(IEnumerable<T> theDataToUpdate) where T : class
        {
            try
            {

                foreach (var modifiedEntity in theDataToUpdate)
                {
                    this.db.Set<T>().Attach(modifiedEntity);
                    this.db.Entry(modifiedEntity).State = EntityState.Modified;
                }

                var noOfSavedRecords = this.db.SaveChangesAsync();
                return noOfSavedRecords.Result;

            }
            catch (Exception ex)
            {

                throw new Exception("There was a problem updating records to the DB using RevelDBWriter", ex);
            }
        }



    }
}
