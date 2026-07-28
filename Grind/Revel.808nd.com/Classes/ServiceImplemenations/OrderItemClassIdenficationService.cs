using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class OrderItemClassIdentificationService
    {
        ProductClassService _classService;
        IEnumerable<Product> _allProducts;
        IEnumerable<ProductClass> _allProductClasses;

        public OrderItemClassIdentificationService(ProductClassService classService, IEnumerable<Product> allProducts, IEnumerable<ProductClass> allProductClasses)
        {
            _classService = classService;
            _allProducts = allProducts;
            _allProductClasses = allProductClasses;
        }


        public string GetItemClassType(OrderItem item, int establishmentID)
        {
            try
            {
                var thisProd = _allProducts.FirstOrDefault(x => x.establishment_id == establishmentID && x.product_id == item.product_id);
                if (thisProd != null)
                {
                    var prodResult = ProductClassService.GetParentRootClass(thisProd, _allProductClasses);
                    return prodResult.name;
                }

                throw new Exception("Unable To Identify Product Classs");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public IEnumerable<OrderItem> GetProductsThatAreHotDrinksClass(IEnumerable<OrderItem> items, int establishmentID)
        //{
        //    try
        //    {
        //        List<OrderItem> returnedItemsThatMatch = new List<OrderItem>();
        //        foreach (var item in items)
        //        {
        //            if (GetItemClassType(item, establishmentID) == "")
        //                returnedItemsThatMatch.Add(item);
        //        }

        //        return returnedItemsThatMatch;
               
        //    }
        //    catch (Exception ex)
        //    {
        //        //suppress for now

        //        //throw ex;
        //    }
        //}


    }
}
