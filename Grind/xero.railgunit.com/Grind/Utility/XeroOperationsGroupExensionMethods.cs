using Revel._808nd.com.OperationsReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xero.railgunit.com.Grind.Utility
{
    public static class XeroOperationsGroupExensionMethods
    {
        /// <summary>
        /// For Operations Report - Intialiser        /// 
        /// </summary>
        /// <param name="productMixRoot"></param>
        /// <returns></returns>
        public static XeroOperationsProducClassGroupContainer CreateOperationsReportGroup(this RootObject operationsMixRoot)
        {

            var allParentCats = operationsMixRoot.product_mix_data.Select(x => x.parent_pclass).Distinct().ToList();
            var toReturn = new List<XeroOperationsProducClassGroup>();
            var salesData = operationsMixRoot.sales_data;


            allParentCats.ForEach(x =>
            {
                var prods = operationsMixRoot.product_mix_data.Where(y => y.parent_pclass == x).First();
                var grouping = new XeroOperationsProducClassGroup { ParentCategoryName = x, ProductMix = prods }; //each group has an identical copy of the sales data

                toReturn.Add(grouping);
            });


            return new XeroOperationsProducClassGroupContainer { SalesData = salesData, XeroOperationsProducClassGroups = toReturn };

        }


        /// <summary>
        /// Get Total Net Sales
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetTotalGrossSales(this XeroOperationsProducClassGroup productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.ProductMix.price);
        }


        /// <summary>
        /// Returns Gross Sales For that Category
        /// </summary>
        public static decimal GetTotalNetSales(this XeroOperationsProducClassGroup productClassGroup)
        {
            throw new NotImplementedException();
            //return Convert.ToDecimal(productClassGroup.ProductMix.price);
        }


        /// <summary>
        /// Returns Sum Total Taxed Sales For that Category
        /// </summary>
        public static decimal GetTotalTaxedSales(this XeroOperationsProducClassGroup productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.ProductMix.taxable_sales);
        }

        /// <summary>
        /// Returns Sum Total Tax Amount For that Category
        /// </summary>
        public static decimal GetTotalTaxAmount(this XeroOperationsProducClassGroup productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.ProductMix.tax);
        }


        /// <summary>
        /// Returns total non taxed sales
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetTotalNonTaxedSales(this XeroOperationsProducClassGroup productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.ProductMix.untaxable_sales);
        }


        //QTY METHODS
        /// <summary>
        /// Total Item Qty
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static int GetTotalQty(this XeroOperationsProducClassGroup productClassGroup)
        {
            return Convert.ToInt16(productClassGroup.ProductMix.n_items);
        }

        /// <summary>
        /// Total Voids
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static int GetTotalVoids(this XeroOperationsProducClassGroup productClassGroup)
        {
            return Convert.ToInt16(productClassGroup.ProductMix.n_voids);
        }

        /// <summary>
        /// Total Comps
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static int GetTotalComps(this XeroOperationsProducClassGroup productClassGroup)
        {
            return Convert.ToInt16(productClassGroup.ProductMix.n_comps);
        }

        //Discounts
        /// <summary>
        /// Get Item Discounts
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetItemDiscounts(this XeroOperationsProducClassGroup productClassGroup)
        {
            return productClassGroup.ProductMix.discount;
        }

        /// <summary>
        /// Get Order Discounts
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetOrderDiscounts(this XeroOperationsProducClassGroup productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.ProductMix.order_discount);
        }


        /// <summary>
        /// Get Tips - Used by Soho Grind
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetTips(this XeroOperationsProducClassGroupContainer productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.SalesData.tips_total);
        }


        /// <summary>
        /// Get Service Fee
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetServiceFee(this XeroOperationsProducClassGroupContainer productClassGroup)
        {
            return Convert.ToDecimal(productClassGroup.SalesData.service_fee_total);
        }

        /// <summary>
        /// Get Combined Gift And Service Totals
        /// </summary>
        /// <param name="productClassGroup"></param>
        /// <returns></returns>
        public static decimal GetGiftAndServicePayable(this XeroOperationsProducClassGroupContainer productClassGroup)
        {

            var giftCardsAmount = Convert.ToDecimal(productClassGroup.SalesData.gift_sales_payable);
            var storeCreditAmount = Convert.ToDecimal(productClassGroup.SalesData.store_credit_sales_payable);
            var total = giftCardsAmount + storeCreditAmount;

            return total;
        }




    }
}
