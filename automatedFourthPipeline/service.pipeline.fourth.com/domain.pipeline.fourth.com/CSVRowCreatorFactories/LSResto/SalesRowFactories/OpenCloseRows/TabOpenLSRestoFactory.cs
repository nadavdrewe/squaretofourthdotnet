using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using core.lightspeed.com.Models.Core;
using core.lightspeed.com.Models.Financial.Receipts;
using core.lightspeed.com.Models.Labour;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.OpenCloseRows
{
    public static class TabOpenLSRestoFactory
    {
        public static TransactionDatasetRow Create(
        Employee employee,
        Receipt lsReceipt,
        Reservation reservation,
        DateTime transactionDate,
        string unitId,
        string siteLocationCode,
        string revenueCenter,
        string newRecordActivityCode,
        string terminalCode,
        string terminalDesc
       )
        {
            var tabOpenRow = BaseLSRestoRowFactory.Create(employee,
                lsReceipt,
                reservation,
                transactionDate,
                unitId,
                siteLocationCode, newRecordActivityCode.ToString());

            //now set props for this type and return  
            tabOpenRow.TransactionTypeCode = TransactionTypeCodes.TAB_OPEN;

            tabOpenRow.TerminalCode = terminalCode;
            tabOpenRow.TerminalDesc = terminalDesc;

            //set to default nothing
            //tabOpenRow.Qty = 0;
            //tabOpenRow.ListPrice = 0.00M;
            //tabOpenRow.Tax = 0.00M;
            ////tabOpenRow.Currency = 0.00M;
            //tabOpenRow.ListPrice = 0.00M;
            //tabOpenRow.Tax = 0.00M;
            //tabOpenRow.PricePaid = 0.00M;
            //tabOpenRow.Deduction = 0.00M;
            //tabOpenRow.TenderAmount = 0.00M;
            //tabOpenRow.CostPriceTheo = 0.00M;
            //tabOpenRow.ListPriceConv = 0.00M;
            //tabOpenRow.TaxConv = 0.00M;
            //tabOpenRow.PricePaidConv = 0.00M;
            //tabOpenRow.DeductionConv = 0.00M;
            //tabOpenRow.TenderAmountConv = 0.00M;
            //tabOpenRow.CostPriceTheoConv = 0.00M;

            //continue setting props
            //tabOpenRow.OrderTypeDesc = "WHEREDOESTHISCOMEFROM?";
            //tabOpenRow.TabOwner = "NEEDSTOBESET";
            //tabOpenRow.TabOwnerDesc = "NEEDSTOBESET";
            //tabOpenRow.OriginalTabOwner = "NEEDSTOBESET";
            //tabOpenRow.OriginalTabOwnerDesc = "NEEDSTOBESET";
            tabOpenRow.TransactionStartEnd = TransactionStartEndCodes.Start; //1
            tabOpenRow.IsDeleted = "FALSE";

            return tabOpenRow;
        }
    }
}
