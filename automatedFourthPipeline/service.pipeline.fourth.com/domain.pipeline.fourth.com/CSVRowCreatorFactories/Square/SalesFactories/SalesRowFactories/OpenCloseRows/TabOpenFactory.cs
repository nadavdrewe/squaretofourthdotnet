using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using Square;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.SalesFactories.SalesRowFactories
{
    public static class TabOpenRowFactory
    {
        public static TransactionDatasetRow Create(
            TeamMember employee,
            Order squareOrder,
          string unitId,
          string siteLocationCode,
          string newRecordActivityCode,
          string terminalCode,
          string terminalDesc
         )
        {
            var tabOpenRow = BaseSquareRowFactory.Create(employee, squareOrder, unitId, siteLocationCode, newRecordActivityCode.ToString());

            //now set props for this type and return
            tabOpenRow.TransactionTypeCode = TransactionTypeCodes.TAB_OPEN;

            tabOpenRow.TerminalCode = terminalCode;
            tabOpenRow.TerminalDesc = terminalDesc;

            tabOpenRow.TransactionStartEnd = TransactionStartEndCodes.Start; //1
            tabOpenRow.IsDeleted = "FALSE";

            return tabOpenRow;
        }
    }
}
