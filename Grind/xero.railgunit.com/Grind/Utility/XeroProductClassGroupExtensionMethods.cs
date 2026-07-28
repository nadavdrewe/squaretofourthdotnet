using Revel._808nd.com.ProductMix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xero.railgunit.com.Grind.Extension
{
    public static class XeroProductMixClassGroupExtensionMethods
    {

        /// <summary>
        /// For Product Mix report
        /// </summary>
        /// <param name="productMixRoot"></param>
        /// <returns></returns>
        public static IList<XeroProductMixClassGroup> GetParentCategoriesAndProductGroups(this RootObject productMixRoot)
        {

            var allParentCats = productMixRoot.productmix.Select(x => x.parent_pclass).Distinct().ToList();
            var toReturn = new List<XeroProductMixClassGroup>();

            allParentCats.ForEach(x =>
            {
                var prods = productMixRoot.productmix.Where(y => y.parent_pclass == x).ToList();
                var grouping = new XeroProductMixClassGroup { ParentCategoryName = x, ProductMixes = prods };

                toReturn.Add(grouping);
            });

            return toReturn;

        }


        /// <summary>
        /// Returns Sum Total Taxed Sales For that Category
        /// </summary>
        public static decimal GetTotalTaxedSales(this XeroProductMixClassGroup productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.ProductMixes.Sum(x => Convert.ToDecimal(x.taxable_sales)));
        }

        /// <summary>
        /// Returns Sum Total Tax Amount For that Category
        /// </summary>
        public static decimal GetTotalTaxAmount(this XeroProductMixClassGroup productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.ProductMixes.Sum(x => Convert.ToDecimal(x.tax)));
        }


        /// <summary>
        /// Returns total non taxed sales
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetTotalNonTaxedSales(this XeroProductMixClassGroup productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.ProductMixes.Sum(x => Convert.ToDecimal(x.untaxable_sales)));
        }


        //QTY METHODS
        /// <summary>
        /// Total Item Qty
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static int GetTotalQty(this XeroProductMixClassGroup productClassGroup)
        {
            return productClassGroup.ProductMixes.Sum(x => Convert.ToInt16(x.n_items));
        }

        /// <summary>
        /// Total Voids
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static int GetTotalVoids(this XeroProductMixClassGroup productClassGroup)
        {
            return productClassGroup.ProductMixes.Sum(x => Convert.ToInt16(x.n_voids));
        }

        /// <summary>
        /// Total Comps
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static int GetTotalComps(this XeroProductMixClassGroup productClassGroup)
        {
            return productClassGroup.ProductMixes.Sum(x => Convert.ToInt16(x.n_comps));
        }

        //Discounts
        /// <summary>
        /// Get Item Discounts
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetItemDiscounts(this XeroProductMixClassGroup productClassGroup)
        {
            return productClassGroup.ProductMixes.Sum(x => Convert.ToDecimal(x.discount));
        }

        /// <summary>
        /// Get Order Discounts
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetOrderDiscounts(this XeroProductMixClassGroup productClassGroup)
        {
            return productClassGroup.ProductMixes.Sum(x => Convert.ToDecimal(x.order_discount));
        }



     

    }
}
