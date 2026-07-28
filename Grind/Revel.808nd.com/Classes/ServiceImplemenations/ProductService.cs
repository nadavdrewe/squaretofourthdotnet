using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class ProductService : BaseService
    {
        public ProductService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {
        }

        private static class ProductQueries
        {
            public static string getAllProducts { get; } = "/resources/Products/?format=json&limit=700";
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await this._webReader.GetRevelWebserviceData<Product>(new Product(), ProductService.ProductQueries.getAllProducts, _genericObjectCreatorFactory);
        }

    }
}
