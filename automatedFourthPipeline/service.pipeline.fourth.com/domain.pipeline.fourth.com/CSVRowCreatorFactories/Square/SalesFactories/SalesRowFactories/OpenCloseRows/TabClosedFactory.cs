using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using Square;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.SalesFactories
{
    public static class TabClosedRowFactory
    {
        public static TransactionDatasetRow Create(TeamMember employuee, Order squareOrder,
          string unitId,
          string siteLocationCode,
          string newRecordActivityCode,
          string receiptCode,
          string checkCode,

        string terminalCode,
        string terminalDesc)
        {
            var tabClosedRow = BaseSquareRowFactory.Create(employuee, squareOrder, unitId, siteLocationCode, newRecordActivityCode.ToString());

            //now set props for this type and return
            tabClosedRow.TransactionTypeCode = TransactionTypeCodes.TAB_CLOSE;

            tabClosedRow.TerminalCode = terminalCode;
            tabClosedRow.TerminalDesc = terminalDesc;

            tabClosedRow.TransactionStartEnd = TransactionStartEndCodes.End; //1
            tabClosedRow.IsDeleted = "FALSE";

            return tabClosedRow;
        }
    }
}
