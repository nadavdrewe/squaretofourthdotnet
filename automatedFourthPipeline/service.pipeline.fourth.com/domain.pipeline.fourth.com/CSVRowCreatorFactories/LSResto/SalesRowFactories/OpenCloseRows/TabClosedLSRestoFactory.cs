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
    public static class TabClosedLSRestoFactory
    {
        public static TransactionDatasetRow Create(Employee lsemployee,
        Receipt lsReceipt,
        Reservation reservation,
        DateTime transactionDate,
        string unitId,
        string siteLocationCode,
        string revenueCenter,
        string newRecordActivityCode,       
        string terminalCode,
        string terminalDesc)
        {

            var tabClosedRow = BaseLSRestoRowFactory.Create(lsemployee,
                         lsReceipt,
                         reservation,
                         transactionDate,
                         unitId,
                         siteLocationCode,
                         newRecordActivityCode.ToString(),
                         "",
                         ""
                        );

            //now set props for this type and return           
            tabClosedRow.TransactionTypeCode = TransactionTypeCodes.TAB_CLOSE;

            tabClosedRow.TerminalCode = terminalCode;
            tabClosedRow.TerminalDesc = terminalDesc;

            //set to default nothing            
            //tabClosedRow.Qty = "0";
            //tabClosedRow.ListPrice = 0.00M;
            //tabClosedRow.Tax = 0.00M;
            ////tabClosedRow.Currency = 0.00M;
            //tabClosedRow.ListPrice = 0.00M;
            //tabClosedRow.Tax = 0.00M;
            //tabClosedRow.PricePaid = 0.00M;
            //tabClosedRow.Deduction = 0.00M;
            //tabClosedRow.TenderAmount = 0.00M;
            //tabClosedRow.CostPriceTheo = 0.00M;
            //tabClosedRow.ListPriceConv = 0.00M;
            //tabClosedRow.TaxConv = 0.00M;
            //tabClosedRow.PricePaidConv = 0.00M;
            //tabClosedRow.DeductionConv = 0.00M;
            //tabClosedRow.TenderAmountConv = 0.00M;
            //tabClosedRow.CostPriceTheoConv = 0.00M;

            ////continue setting props
            //tabClosedRow.OrderTypeDesc = "WHEREDOESTHISCOMEFROM?";
            //tabClosedRow.TabOwner = "NEEDSTOBESET";
            //tabClosedRow.TabOwnerDesc = "NEEDSTOBESET";
            //tabClosedRow.OriginalTabOwner = "NEEDSTOBESET";
            //tabClosedRow.OriginalTabOwnerDesc = "NEEDSTOBESET";
            tabClosedRow.TransactionStartEnd = TransactionStartEndCodes.End; //1
            tabClosedRow.IsDeleted = "FALSE";

            return tabClosedRow;
        }
    }
}
