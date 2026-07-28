using domain.pipeline.fourth.com.Helper;
using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using Square;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Square.SalesFactories.SalesRowFactories.DiscountAndModifierRows
{
    public static class ModifierRowFactory
    {
        public static TransactionDatasetRow Create(TeamMember employee, Order squareOrder,
     OrderLineItem item,
     OrderLineItemModifier modifier,
      string unitId,
      string siteLocationCode,
      string newRecordActivityCode,
      string receiptCode,
      string checkCode,


      string terminalCode,
      string terminalDesc,
      string categoryName)
        {
            var modifierRow = BaseSquareRowFactory.Create(employee, squareOrder, unitId, siteLocationCode, newRecordActivityCode.ToString());

            //now set props for this type and return
            modifierRow.TransactionTypeCode = TransactionTypeCodes.MODIFIER_ITEM;

            //DO SOME CALCS - NEEDS TO BE IN POUNDS AND PENCE - SOURCE IS PENCE
            var listPrice = Convert.ToDecimal(modifier.BasePriceMoney.Amount).DivideBy100();

            ////NOW SET ITEM ONCE CALCS ARE COMPLETE
            ////set terminal desc - these come from 'Device'
            modifierRow.TerminalCode = terminalCode;
            modifierRow.TerminalDesc = terminalDesc;

            //modifierRow.SalesItemPLU = catalogObject.ItemVariationData.Sku;
            modifierRow.SalesItemGUID = item.Uid;
            modifierRow.SalesItemDesc = modifier.Name;

            //set category - all same - single cat in Square
            modifierRow.MajorGroupDesc = categoryName;
            modifierRow.FamilyGroupDesc = categoryName;
            modifierRow.SubGroupDesc = categoryName;

            modifierRow.Qty = "1";
            modifierRow.ListPrice = listPrice; //is this x qty
            modifierRow.Currency = item.BasePriceMoney.Currency?.ToString() ?? "";

            //continue setting props
            modifierRow.TransactionStartEnd = TransactionStartEndCodes.None;
            modifierRow.IsDeleted = "FALSE";

            return modifierRow;
        }
    }
}
