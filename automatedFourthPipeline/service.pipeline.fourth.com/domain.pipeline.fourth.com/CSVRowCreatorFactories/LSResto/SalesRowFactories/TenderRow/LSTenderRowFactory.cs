using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using core.lightspeed.com.Models.Core;
using core.lightspeed.com.Models.Financial.Receipts;
using core.lightspeed.com.Models.Labour;
using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.TenderRow
{
    public static class LSTenderRowFactory
    {
        public static TransactionDatasetRow Create(
        Employee lsemployee,
        Receipt lsReceipt,
        Reservation reservation,
        Payment lsPayment,
        DateTime transactionDate,
        string unitId,
        string siteLocationCode,
        string newRecordActivityCode,

        string terminalCode,
        string terminalDesc,
        string currency)
        {

            var tenderRow = BaseLSRestoRowFactory.Create(lsemployee,
                lsReceipt,
                reservation,
                transactionDate,
                unitId,
                siteLocationCode,
                newRecordActivityCode.ToString(), "",
                ""
               );
            var tenderAmount = Convert.ToDecimal(lsPayment.amount);

            //now set props for this type and return            
            tenderRow.TransactionTypeCode = TransactionTypeCodes.TENDER;

            tenderRow.Currency = currency;
            tenderRow.TenderAmount = tenderAmount;
            tenderRow.TenderAmountConv = tenderAmount;
            tenderRow.TenderTypeDesc = lsPayment.type;
            tenderRow.TenderTypeCode = lsPayment.type;

            tenderRow.TerminalCode = terminalCode;
            tenderRow.TerminalDesc = terminalDesc;

            //continue setting props            
            tenderRow.TransactionStartEnd = TransactionStartEndCodes.None;
            tenderRow.IsDeleted = "FALSE";

            return tenderRow;
        }
    }
}
