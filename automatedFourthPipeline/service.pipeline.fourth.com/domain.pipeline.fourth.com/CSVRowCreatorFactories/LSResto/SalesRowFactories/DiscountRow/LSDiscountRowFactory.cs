using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using core.lightspeed.com.Models.Core;
using core.lightspeed.com.Models.Financial.Receipts;
using core.lightspeed.com.Models.Labour;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.DiscountRow
{
    public static class LSDiscountRowFactory
    {
        public static TransactionDatasetRow Create(
            Employee lsemployee,
            Receipt lsReceipt,
            Reservation reservation,
            Item discount,
            Item linkedItemDiscountIsFor, //Might be order level discount and is null
            DateTime transactionDate,
              string unitId,
              string siteLocationCode,
              string newRecordActivityCode,


              string terminalCode,
              string terminalDesc,
                     string categoryName,
                     string currency)
        {
            var discountRow = BaseLSRestoRowFactory.Create(lsemployee,
               lsReceipt,
               reservation,
               transactionDate,
               unitId,
               siteLocationCode,
               newRecordActivityCode.ToString(),
               "",
               ""
              );


            var discountAmount = discount.totalPrice * -1;

            //now set props for this type and return            
            discountRow.TransactionTypeCode = TransactionTypeCodes.DISC_ITEM;

            //discount
            discountRow.DeductionCode = discount.prodId;
            discountRow.DeductionDesc = discount.productName;
            discountRow.Deduction = discountAmount;
            discountRow.DeductionConv = discountAmount;

            //set terminal desc - these come from 'Device'
            discountRow.TerminalCode = terminalCode;
            discountRow.TerminalDesc = terminalDesc;

            //set item details for this discount 
            discountRow.SalesItemPLU = linkedItemDiscountIsFor?.productPLU ?? "";
            discountRow.SalesItemGUID = "";
            discountRow.SalesItemDesc = linkedItemDiscountIsFor?.productName ?? "";

            discountRow.MajorGroupDesc = categoryName;
            discountRow.FamilyGroupDesc = categoryName;
            discountRow.SubGroupDesc = categoryName;

            discountRow.Currency = currency;

            //set to default nothing
            discountRow.TenderAmount = 0.00M;
            discountRow.CostPriceTheo = 0.00M;
            discountRow.ListPriceConv = 0.00M;
            discountRow.TaxConv = 0.00M;
            discountRow.PricePaidConv = 0.00M;          
            //continue setting props       
            discountRow.TransactionStartEnd = TransactionStartEndCodes.None;
            discountRow.IsDeleted = "FALSE";

            return discountRow;
        }
    }
}
