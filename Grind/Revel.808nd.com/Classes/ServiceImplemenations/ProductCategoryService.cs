using Revel._808nd.com.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class ProductCategoryService
    {
        IEnumerable<ProductCategory> _allAvailableCategories;

        public ProductCategoryService(IEnumerable<ProductCategory> allAvailableCategories)
        {
            _allAvailableCategories = allAvailableCategories;
        }



        public ProductCategory GetParentCategory(Product product)
        {
            try
            {
                ProductCategory parentCategory;
                ProductCategory loopCategory;
                List<ProductCategory> restrictedCategories;

                if (product.resource_uri == "/resources/Product/18284/")
                {
                    var tsop = "";
                }



                try
                {
                    var firstCategory = _allAvailableCategories.FirstOrDefault(x => x.productcategory_id == product.categoryID);
                    loopCategory = firstCategory;
                    restrictedCategories = _allAvailableCategories.Where(x => x.establishment_id == firstCategory.establishment_id).ToList(); //LIMIT FOR ESTABLISHMENT

                }
                catch (Exception ex)
                {

                    throw ex;
                }

                try
                {
                    while (loopCategory.parent_id != 0)
                    {
                        loopCategory = restrictedCategories.Where(x => x.productcategory_id == loopCategory.parent_id).FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

                return loopCategory;

            }
            catch (Exception ex)
            {

                throw new Exception("Couldn't identify parent category for product" + product.resource_uri, ex);
            }


        }

    }
}
