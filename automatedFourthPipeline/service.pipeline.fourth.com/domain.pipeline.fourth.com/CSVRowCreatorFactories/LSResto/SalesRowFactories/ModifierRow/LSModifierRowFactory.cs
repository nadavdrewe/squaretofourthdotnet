using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using core.lightspeed.com.CreatorFactories.OnlineOrdering;
using core.lightspeed.com.Models.Core;
using core.lightspeed.com.Models.Financial.Receipts;
using core.lightspeed.com.Models.Inventory;
using core.lightspeed.com.Models.Labour;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.ModifierRow
{
    public static class LSModifierRowFactory
    {
        public static TransactionDatasetRow Create(
            Employee lsemployee,
            Receipt lsReceipt,
            Reservation reservation,
            core.lightspeed.com.Models.Financial.Receipts.Item item,
            ModifierValue2 modifier,
            Product modifierProd,
            DateTime transactionDate,
            string unitId,
            string siteLocationCode,
            string newRecordActivityCode,
            string terminalCode,
            string terminalDesc,
            string categoryName,
            string currency)
        {
            var modifierRow = BaseLSRestoRowFactory.Create(lsemployee,
                 lsReceipt,
                 reservation,
                 transactionDate,
                 unitId,
                 siteLocationCode,
                 newRecordActivityCode.ToString(), "",
                 ""
                );

            //now set props for this type and return            
            modifierRow.TransactionTypeCode = TransactionTypeCodes.MODIFIER_ITEM;

            //DO SOME CALCS - NEEDS TO BE IN POUNDS AND PENCE - SOURCE IS PENCE
            var listPrice = modifier.price;
            var tax = modifier.price - modifier.priceWithoutVAT;
            //var pricePaid = Convert.ToDecimal(modifier.TotalMoney.Amount).TimesAHundred();
            //var deducation = Convert.ToDecimal(modifier.TotalDiscountMoney.Amount).TimesAHundred();

            ////NOW SET ITEM ONCE CALCS ARE COMPLETE
            ////set terminal desc - these come from 'Device'
            modifierRow.TerminalCode = terminalCode;
            modifierRow.TerminalDesc = terminalDesc;

            modifierRow.SalesItemPLU = modifier.plu;
            modifierRow.SalesItemId = modifier.modifierId.ToString();
            modifierRow.SalesItemDesc = modifier.name;

            //set category - all same - single cat in Square
            modifierRow.MajorGroupDesc = categoryName;
            modifierRow.FamilyGroupDesc = categoryName;
            modifierRow.SubGroupDesc = categoryName;

            modifierRow.Qty = "1";
            modifierRow.ListPrice = listPrice; //is this x qty
            modifierRow.Tax = tax;
            modifierRow.Currency = currency;
            //tenderRow.PricePaid = pricePaid;
            //tenderRow.Deduction = deducation;

            //tenderRow.CostPriceTheo = 0.00M;
            //tenderRow.ListPriceConv = listPrice;
            //tenderRow.TaxConv = tax;
            //tenderRow.PricePaidConv = pricePaid;
            //tenderRow.DeductionConv = deducation;            
            //tenderRow.CostPriceTheoConv = 0.00M;

            //continue setting props            
            modifierRow.TransactionStartEnd = TransactionStartEndCodes.None;
            modifierRow.IsDeleted = "FALSE";

            return modifierRow;
        }
    }
}
