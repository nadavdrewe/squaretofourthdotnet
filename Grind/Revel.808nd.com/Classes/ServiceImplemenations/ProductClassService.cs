using domain.artistresidence.railgunit.com.DataContext;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class ProductClassService : BaseService
    {
        /// <summary>
        /// Contains query strings for Revel Queries
        /// </summary>
        public static class ProductClassServiceQueries
        {
            public static string getAllProductClasses = "products/ProductClass/?format=json&limit=0";
        }

        public ProductClassService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {
        }

        public async Task<IEnumerable<ProductClass>> GetProductClassesFromRevel(string query)
        {
            return await this._webReader.GetRevelWebserviceData<ProductClass>(new ProductClass(), query, _genericObjectCreatorFactory);
        }

        public int ReplaceOrInsert(IEnumerable<ProductClass> productClasses)
        {
            try
            {
                if (productClasses.Count() > 0)
                {

                    var toReplace = new List<ProductClass>();
                    foreach (var pClass in productClasses)
                    {
                        var existing = _db.ProductClasses.FirstOrDefault(x => x.id == pClass.id);
                        if (existing != null)
                        {
                            toReplace.Add(existing);
                        }
                    }

                 ((DbSet<ProductClass>)_db.ProductClasses).RemoveRange(toReplace);
                    ((DbSet<ProductClass>)_db.ProductClasses).AddRange(productClasses);
                    return _db.SaveChanges();
                }

                return 0;

            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't save ProductClasses to local DB", ex);
            }
        }


        public async Task GetAllProductClassesAndReplaceLocal()
        {
            var query = ProductClassService.ProductClassServiceQueries.getAllProductClasses;
            var newItems = await GetProductClassesFromRevel(query);
            ReplaceOrInsert(newItems);

        }


        public static ProductClass GetParentRootClass(Product product, IEnumerable<ProductClass> allAvailableClasses)
        {
            try
            {
                ProductClass parentClass ;
                ProductClass loopClass;
                List<ProductClass> restrictedClasses;
                

                try
                {
                    var firstCategory = allAvailableClasses.FirstOrDefault(x => x.resource_uri == product.productclass);
                    loopClass = firstCategory;                   

                }
                catch (Exception ex)
                {

                    throw ex;
                }

                try
                {
                    while (!String.IsNullOrWhiteSpace(loopClass.parent))
                    {
                        loopClass = allAvailableClasses.Where(x => x.resource_uri == loopClass.parent).FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

                return loopClass;

            }
            catch (Exception ex)
            {

                throw new Exception("Couldn't identify parent category for product" + product.resource_uri, ex);
            }
        }


        public static ProductClass GetParentRootClass(Product product, ArtistsResidenceContext db)
        {
            try
            {
                ProductClass parentClass;
                ProductClass loopClass;
                List<ProductClass> restrictedClasses;


                try
                {
                    var firstCategory = db.ProductClasses.FirstOrDefault(x => x.resource_uri == product.productclass);
                    loopClass = firstCategory;

                }
                catch (Exception ex)
                {

                    throw ex;
                }

                try
                {
                    while (!String.IsNullOrWhiteSpace(loopClass.parent))
                    {
                        loopClass = db.ProductClasses.Where(x => x.resource_uri == loopClass.parent).FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

                return loopClass;

            }
            catch (Exception ex)
            {

                throw new Exception("Couldn't identify parent category for product" + product.resource_uri, ex);
            }


        }


    }
}
