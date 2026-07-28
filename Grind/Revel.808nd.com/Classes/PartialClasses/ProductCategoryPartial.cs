using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{

    public partial class ProductCategory
    {

        public static async Task<List<ProductCategory>> CompareProductCategoriesAndInsertNewIntoDB(IRevelReaderAsync readerDB,
            IRevelReaderAsync readerWebservice, IRevelWriter writer)
        {
            try
            {              

                var allCats = await readerWebservice.GetProductCategoriesNoEstablishment();
                var ExistingCats = await readerDB.GetProductCategoriesNoEstablishment();


                var ordered = allCats.OrderBy(x => x.productcategory_id).ToList();


                var newCats = new List<ProductCategory>();
                List<int> newCatIDs = new List<int>();

                var allcatIDs = allCats.Select(x => x.productcategory_id).ToList();
                var existingCatIDs = ExistingCats.Select(x => x.productcategory_id).ToList();

                newCatIDs = allcatIDs.Except(existingCatIDs).ToList();

                if (newCatIDs.Any())
                {
                    //add all new prodcats to the list
                    foreach (var productCategoryID in newCatIDs)
                    {
                        var catToAdd = allCats.Where(c => c.productcategory_id == productCategoryID).First();
                        newCats.Add(catToAdd);
                    }

                    using (var _db = new GrindContext())
                    {
                        _db.ProductCategories.AddRange(newCats);
                        _db.SaveChanges();

                    }
                    
                    //save em

                }

                //return what we've added 
                return newCats;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
