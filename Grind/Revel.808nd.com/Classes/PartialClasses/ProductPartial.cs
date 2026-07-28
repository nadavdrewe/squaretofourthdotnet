using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{
    public partial class Product : IRevelCreateable
    {
        public static async Task<List<Product>> CompareProductsDeleteOldAndInsertNewIntoDB(IRevelReaderAsync readerDB, IRevelReaderAsync readerWebservice, IRevelWriter writer)
        {
            try
            {
                List<Product> newProducts = new List<Product>();


                //do compare
                var AllProds = await readerWebservice.GetProductsNoEstablishment();
                var ExistingProducts = await readerDB.GetProductsNoEstablishment();

                /*
                                var newProds = new List<Product>();
                                List<int> newProdIDs = new List<int>();

                                var allProdIDs = AllProds.Select(x => x.product_id).ToList();
                                var existingProdIDs = ExistingProducts.Select(x => x.product_id).ToList();


                                newProdIDs = allProdIDs.Except(existingProdIDs).ToList();*/

                using (var _db = new GrindContext())
                {


                    //add all new prodcats to the list
                    /*   foreach (var productID in newProdIDs)
                       {
                           var prodToAdd = AllProds.Where(c => c.product_id == productID).First();
                           newProds.Add(prodToAdd);
                       }
   */
                    foreach (var prod in ExistingProducts)
                    {
                        
                        _db.Products.Attach(prod);
                        _db.Products.Remove(prod);
                    }
                    _db.SaveChanges();

                    _db.Products.AddRange(AllProds);
                    _db.SaveChanges();                  

                }




                return newProducts;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public static List<int> GetIntegerFromPrimaryKey(List<Product> products)
        {
            List<int> prodIDs = new List<int>();

            foreach (var product in products)
            {

                prodIDs.Add(product.product_id);
            }

            return prodIDs;

        }

        public int Create(dynamic Type)
        {
            brand = (string)Type["brand"];
            product_id = (int)Type["id"];
            name = (string)Type["name"];

            price = Convert.ToDecimal(RevelHelper.CheckIfJSONZeroAndReturnZeroDecimalString((string)Type["price"]));
            sku = (string)Type["sku"];
            //   tax_included = (bool)Type["tax_included"];
            //   tax = (string)Type["tax"];
            //  tax_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(tax);
            active = (string)Type["active"];

            establishment = (string)Type["establishment"];
            establishment_id =
                    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                        (string)Type["establishment"]);

            category = (string)Type["category"];
            categories = ((string)Type["category"]).Split(';');
            category_ids = new List<int?>();
            productclass = (string)Type["productclass"];
            categoryID = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)Type["category"]);

            resource_uri = (string)Type["resource_uri"];

            return 0;
        }
    }
}
