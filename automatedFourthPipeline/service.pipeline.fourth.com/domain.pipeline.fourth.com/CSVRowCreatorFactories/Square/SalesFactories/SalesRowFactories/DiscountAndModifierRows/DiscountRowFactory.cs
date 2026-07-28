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
    public static class DiscountRowFactory
    {
        public static TransactionDatasetRow Create(TeamMember employee, Order squareOrder,
    OrderLineItem item,
    CatalogObjectItemVariation catalogObject, //this is the item variation
    OrderLineItemDiscount discount,
     string unitId,
     string siteLocationCode,
     string newRecordActivityCode,
     string receiptCode,
     string checkCode,

     string terminalCode,
     string terminalDesc,
            string categoryName)
        {
            var discountRow = BaseSquareRowFactory.Create(employee, squareOrder, unitId, siteLocationCode, newRecordActivityCode.ToString());


            var discountAmount = Convert.ToDecimal(discount.AppliedMoney.Amount).DivideBy100();

            //now set props for this type and return
            discountRow.TransactionTypeCode = TransactionTypeCodes.DISC_ITEM;

            //discount
            discountRow.DeductionCode = discount.CatalogObjectId;
            discountRow.DeductionDesc = discount.Name;
            discountRow.Deduction = Convert.ToDecimal(discount.AppliedMoney.Amount) / 100.00M;
            discountRow.DeductionConv = 0.00M;

            //set terminal desc - these come from 'Device'
            discountRow.TerminalCode = terminalCode;
            discountRow.TerminalDesc = terminalDesc;

            //set item details for this discount
            discountRow.SalesItemPLU = catalogObject?.ItemVariationData?.Sku ?? "";
            discountRow.SalesItemGUID = item.Uid;
            discountRow.SalesItemDesc = item.Name + " - " + item.VariationName;

            discountRow.MajorGroupDesc = categoryName;
            discountRow.FamilyGroupDesc = categoryName;
            discountRow.SubGroupDesc = categoryName;

            discountRow.Currency = discount.AppliedMoney.Currency?.ToString() ?? "";

            //set to default nothing
            discountRow.TenderAmount = 0.00M;
            discountRow.CostPriceTheo = 0.00M;
            discountRow.ListPriceConv = 0.00M;
            discountRow.TaxConv = 0.00M;
            discountRow.PricePaidConv = 0.00M;
            discountRow.DeductionConv = 0.00M;


            //continue setting props
            discountRow.TransactionStartEnd = TransactionStartEndCodes.None;
            discountRow.IsDeleted = "FALSE";

            return discountRow;
        }
    }
}
