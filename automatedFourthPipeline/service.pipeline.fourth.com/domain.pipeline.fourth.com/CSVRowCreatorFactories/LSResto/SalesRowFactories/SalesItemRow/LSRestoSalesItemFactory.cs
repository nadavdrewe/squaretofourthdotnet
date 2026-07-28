using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using core.lightspeed.com.Models.Core;
using core.lightspeed.com.Models.Financial.Receipts;
using core.lightspeed.com.Models.Inventory;
using core.lightspeed.com.Models.Labour;
using domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.SalesItemRow
{
    public static class LSRestoSalesItemFactory
    {
        public static TransactionDatasetRow Create(
          //base row items
          Employee lsemployee, //
          Receipt lsReceipt,
          Reservation reservation,
          Item lsItem,
          Product product, //this is the variation
          IEnumerable<Item> anyDiscountItems,
          DateTime transactionDate,
          string unitId,
          string siteLocationCode,
          string newRecordActivityCode,
          string terminalCode,
          string terminalDesc,
          string categoryName1,
          string categoryName2,
          string categoryName3,
          string currency
          )
        {
            var salesItemRow = BaseLSRestoRowFactory.Create(lsemployee,
                lsReceipt,
                reservation,
                transactionDate,
                unitId,
                siteLocationCode,
                newRecordActivityCode.ToString(), "",
                ""
               );

            //was ordre takeaway or eat in - know which price to get
            //if there is discount, sum the discount!

            decimal discountPaid = 0.00M;
            if (anyDiscountItems != null && anyDiscountItems.Count() > 0)
            {
                discountPaid = anyDiscountItems.Sum(x => x.totalPrice);
            }


            var listPrice = LSRestoSalesHelper.GetCorrectSalesPriceDependingOnOrderType(product, lsReceipt.type) ?? lsItem.totalPrice;
            var tax = lsItem.totalPrice - lsItem.totalPriceWithoutVat;
            var pricePaid = lsItem.totalPrice - discountPaid; // CAN BE WITH OR WITHOUT TAX,
            var deducation = discountPaid; //needs to be + number for Fourth and LS gives as -

            //NOW SET ITEM ONCE CALCS ARE COMPLETE
            //set terminal desc - these come from 'Device'
            salesItemRow.TerminalCode = terminalCode;
            salesItemRow.TerminalDesc = terminalDesc;

            //now set props for this type and return            
            salesItemRow.TransactionTypeCode = TransactionTypeCodes.SALES_ITEM;

            salesItemRow.SalesItemId = lsItem.id;
            salesItemRow.SalesItemPLU = lsItem.productPLU ?? "UNKNOWN";
            salesItemRow.SalesItemGUID = "";
            salesItemRow.SalesItemDesc = lsItem.productName;

            //set category - all same - single cat in Square
            salesItemRow.MajorGroupDesc = categoryName3;
            salesItemRow.FamilyGroupDesc = categoryName2;
            salesItemRow.SubGroupDesc = categoryName1;

            salesItemRow.Qty = lsItem.amount.ToString();
            salesItemRow.ListPrice = listPrice; //is this x qty
            salesItemRow.Tax = tax;
            salesItemRow.Currency = currency;
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
            //salesItemRow.OrderTypeDesc = "WHEREDOESTHISCOMEFROM?";
            //salesItemRow.TabOwner = "NEEDSTOBESET";
            //salesItemRow.TabOwnerDesc = "NEEDSTOBESET";
            //salesItemRow.OriginalTabOwner = "NEEDSTOBESET";
            //salesItemRow.OriginalTabOwnerDesc = "NEEDSTOBESET";
            salesItemRow.TransactionStartEnd = TransactionStartEndCodes.None;
            salesItemRow.IsDeleted = "FALSE";

            return salesItemRow;
        }
    }
}
