using domain.pipeline.fourth.com.Helper;
using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using Square;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace domain.pipeline.fourth.com.SalesFactories.SalesRowFactories
{
    public static class SalesItemFactory
    {
        public static TransactionDatasetRow Create(
       //base row items
       TeamMember employee,
       Order squareOrder,
       OrderLineItem itemToCreateRowFor,
       CatalogObjectItemVariation catalogObject, //this is the variation
       string unitId,
       string siteLocationCode,
       string newRecordActivityCode,
       string receiptCode,
       string checkCode,
       //
       string terminalCode,
       string terminalDesc,
       string categoryName
       )
        {
            var salesItemRow = BaseSquareRowFactory.Create(employee,
                squareOrder,
                unitId,
                siteLocationCode,
                newRecordActivityCode.ToString()
                );

            //DO SOME CALCS - NEEDS TO BE IN POUNDS AND PENCE - SOURCE IS PENCE
            var modifiersToAdd = 0.00M;
            if (itemToCreateRowFor.Modifiers != null)
            {
                modifiersToAdd = Convert.ToDecimal(itemToCreateRowFor.Modifiers.Where(x => x != null).Sum(x => x.TotalPriceMoney?.Amount ?? x.BasePriceMoney?.Amount ?? 0)).DivideBy100();
            }

            var listPrice = (Convert.ToDecimal(itemToCreateRowFor.BasePriceMoney.Amount) * Convert.ToDecimal(itemToCreateRowFor.Quantity)).DivideBy100() + modifiersToAdd;
            var tax = Convert.ToDecimal(itemToCreateRowFor.TotalTaxMoney.Amount).DivideBy100();
            var pricePaid = Convert.ToDecimal(itemToCreateRowFor.TotalMoney.Amount).DivideBy100();
            var deducation = Convert.ToDecimal(itemToCreateRowFor.TotalDiscountMoney.Amount).DivideBy100();

            //NOW SET ITEM ONCE CALCS ARE COMPLETE
            //set terminal desc - these come from 'Device'
            salesItemRow.TerminalCode = terminalCode;
            salesItemRow.TerminalDesc = terminalDesc;

            //now set props for this type and return
            salesItemRow.TransactionTypeCode = TransactionTypeCodes.SALES_ITEM;

            salesItemRow.SalesItemPLU = catalogObject?.ItemVariationData?.Sku ?? "";
            salesItemRow.SalesItemGUID = itemToCreateRowFor.Uid;
            salesItemRow.SalesItemDesc = itemToCreateRowFor.Name + " - " + itemToCreateRowFor.VariationName;

            //set category - all same - single cat in Square
            salesItemRow.MajorGroupDesc = categoryName;
            salesItemRow.FamilyGroupDesc = categoryName;
            salesItemRow.SubGroupDesc = categoryName;

            salesItemRow.Qty = itemToCreateRowFor.Quantity;
            salesItemRow.ListPrice = listPrice; //is this x qty
            salesItemRow.Tax = tax;
            salesItemRow.Currency = itemToCreateRowFor.BasePriceMoney.Currency?.ToString() ?? "";
            salesItemRow.PricePaid = pricePaid;
            salesItemRow.Deduction = deducation;

            salesItemRow.CostPriceTheo = 0.00M;
            salesItemRow.ListPriceConv = listPrice;
            salesItemRow.TaxConv = tax;
            salesItemRow.PricePaidConv = pricePaid;
            salesItemRow.DeductionConv = deducation;
            //salesItemRow.TenderAmountConv = 0.00M;
            salesItemRow.CostPriceTheoConv = 0.00M;

            //continue setting props
            salesItemRow.TransactionStartEnd = TransactionStartEndCodes.None;
            salesItemRow.IsDeleted = "FALSE";

            return salesItemRow;
        }
    }
}
