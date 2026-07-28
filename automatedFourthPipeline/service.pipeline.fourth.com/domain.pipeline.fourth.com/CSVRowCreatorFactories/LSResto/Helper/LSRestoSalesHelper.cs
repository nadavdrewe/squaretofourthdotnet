using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using core.lightspeed.com.Models.Financial.Receipts;
using core.lightspeed.com.Models.Inventory;
using domain.pipeline.fourth.com.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.Helper
{
    public static class LSRestoSalesHelper
    {
        public static IEnumerable<ProductGroup> CreateUnknownProductGroup()
        {

            return new List<ProductGroup> {
             new ProductGroup
             {
                categoryId = 0,
                id = 0,
                name = "Unknown Group"

            }
            };

        }

        public static Product CreateUnknownProduct()
        {
            return new Product
            {
                id = 0,
                name = "Unknown Product",
                sku = "UNKNOWN"
            };
        }


        public static bool IsItemDiscountItem(Item item)
        {
            if (item.priceTypeId >= 51 && item.priceTypeId < 60)
            {
                //its a discount
                return true;
            }

            return false;
        }

        static bool IsItemServiceChargeItem(Item item)
        {
            if ((item.priceTypeId >= 60 && item.priceTypeId < 67) || (item.priceTypeId >= 70 && item.priceTypeId < 77))
            {
                //its a service charge item
                return true;
            }

            return false;
        }

        public static List<Item> GetDiscountItemsFromListOfItems(List<Item> items)
        {
            var toReturn = new List<Item>();
            foreach (var item in items)
            {
                if (IsItemDiscountItem(item))
                    toReturn.Add(item);
            }

            return toReturn;
        }

        public static List<Item> GetServiceChargeItemsFromListOfItems(List<Item> items)
        {
            var toReturn = new List<Item>();
            foreach (var item in items)
            {
                if (IsItemServiceChargeItem(item))
                    toReturn.Add(item);
            }

            return toReturn;
        }

        public static decimal? GetCorrectSalesPriceDependingOnOrderType(Product product, string receiptType)
        {
            if (product == null)
                return null;

            decimal correctProductPrice = 0.00M;
            switch (receiptType.Trim().ToLower())
            {
                case "bar":
                    correctProductPrice = product.price;
                    break;
                case "delivery":
                    correctProductPrice = product.deliveryPrice;
                    break;
                case "restaurant":
                    correctProductPrice = product.price;
                    break;
                case "tab":
                    correctProductPrice = product.price;
                    break;
                case "takeaway_simple":
                    correctProductPrice = product.takeawayPrice;
                    break;
                case "takeaway":
                    correctProductPrice = product.takeawayPrice;
                    break;
                case "void":
                    correctProductPrice = product.price;
                    break;
                case "voided":
                    correctProductPrice = product.price;
                    break;
                default:
                    string errormsg = $"Couldn't recognise {receiptType} when trying to identify product: {product.name}";
                    throw new UnrecognisedSalesItemException(errormsg);
            }

            return correctProductPrice;
        }

        /// <summary>
        /// Assigns the correct voided type to sales row
        /// </summary>
        /// <param name="alreadyCreatedSalesRow"></param>
        public static void AssignCorrectVoidedTypeToVoidRow(TransactionDatasetRow alreadyCreatedSalesRow, Item originalItem)
        {
            switch (originalItem.info)
            {
                case "Void:Complementary":
                    alreadyCreatedSalesRow.TransactionTypeCode = TransactionTypeCodes.VOID_ITEM;
                    break;
                case "Void:Wastage":
                    alreadyCreatedSalesRow.TransactionTypeCode = TransactionTypeCodes.VOID_WASTE;
                    break;
                case "Void:Staff":
                    alreadyCreatedSalesRow.TransactionTypeCode = TransactionTypeCodes.VOID_ITEM;
                    break;
                default:
                    alreadyCreatedSalesRow.TransactionTypeCode = TransactionTypeCodes.VOID_ITEM;
                    break;
            }
        }
    }
}
