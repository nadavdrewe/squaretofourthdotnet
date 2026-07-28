using domain.pipeline.fourth.com.Helper;
using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using Square;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Square.SalesFactories.SalesRowFactories.TenderRow
{
    public static class TenderSquareRowFactory
    {
        public static TransactionDatasetRow Create(TeamMember employee, Order squareOrder,
        Payment payment,
        Tender actualTender,
        string unitId,
        string siteLocationCode,
        string newRecordActivityCode,

        string terminalCode,
        string terminalDesc)
        {
            var tenderRow = BaseSquareRowFactory.Create(employee, squareOrder, unitId, siteLocationCode, newRecordActivityCode.ToString());

            var tenderAmount = Convert.ToDecimal(actualTender.AmountMoney?.Amount ?? payment?.AmountMoney?.Amount ?? 0).DivideBy100();

            //now set props for this type and return
            tenderRow.TransactionTypeCode = TransactionTypeCodes.TENDER;

            tenderRow.Currency = actualTender.AmountMoney.Currency?.ToString() ?? "";
            tenderRow.TenderAmount = tenderAmount;
            tenderRow.TenderAmountConv = tenderAmount;
            var tenderType = payment?.SourceType ?? actualTender.Type.Value ?? "";
            tenderRow.TenderTypeDesc = tenderType;
            tenderRow.TenderTypeCode = tenderType;

            tenderRow.TerminalCode = terminalCode;
            tenderRow.TerminalDesc = terminalDesc;

            //continue setting props
            tenderRow.TransactionStartEnd = TransactionStartEndCodes.None;
            tenderRow.IsDeleted = "FALSE";

            return tenderRow;
        }

        public static TransactionDatasetRow CreateForRefund(
            TeamMember employee,
            Order squareOrder,
            Refund refund,
            Tender refundedTender,
            string unitId,
            string siteLocationCode,
            string newRecordActivityCode,
            string terminalCode,
            string terminalDesc)
        {
            var tenderRow = BaseSquareRowFactory.Create(employee, squareOrder, unitId, siteLocationCode, newRecordActivityCode.ToString());

            var refundAmount = Convert.ToDecimal(refund.AmountMoney?.Amount ?? 0).DivideBy100() * -1;
            var tenderType = refundedTender != null ? refundedTender.Type.Value : "REFUND";

            tenderRow.TransactionTypeCode = TransactionTypeCodes.TENDER;
            tenderRow.Currency = refund.AmountMoney?.Currency?.ToString() ?? refundedTender?.AmountMoney?.Currency?.ToString() ?? "";
            tenderRow.TenderAmount = refundAmount;
            tenderRow.TenderAmountConv = refundAmount;
            tenderRow.TenderTypeDesc = $"{tenderType} REFUND";
            tenderRow.TenderTypeCode = $"{tenderType}_REFUND";
            tenderRow.TerminalCode = terminalCode;
            tenderRow.TerminalDesc = terminalDesc;
            tenderRow.TransactionStartEnd = TransactionStartEndCodes.None;
            tenderRow.IsDeleted = "FALSE";

            return tenderRow;
        }

        public static TransactionDatasetRow CreateForPaymentRefund(
            TeamMember employee,
            Order squareOrder,
            PaymentRefund refund,
            Payment refundedPayment,
            Tender refundedTender,
            string unitId,
            string siteLocationCode,
            string newRecordActivityCode,
            string terminalCode,
            string terminalDesc)
        {
            var tenderRow = BaseSquareRowFactory.Create(employee, squareOrder, unitId, siteLocationCode, newRecordActivityCode.ToString());

            var refundAmount = Convert.ToDecimal(refund.AmountMoney?.Amount ?? 0).DivideBy100() * -1;
            var tenderType = refundedPayment?.SourceType ?? (refundedTender != null ? refundedTender.Type.Value : "REFUND");

            tenderRow.TransactionTypeCode = TransactionTypeCodes.TENDER;
            tenderRow.Currency = refund.AmountMoney?.Currency?.ToString() ?? refundedTender?.AmountMoney?.Currency?.ToString() ?? "";
            tenderRow.TenderAmount = refundAmount;
            tenderRow.TenderAmountConv = refundAmount;
            tenderRow.TenderTypeDesc = $"{tenderType} REFUND";
            tenderRow.TenderTypeCode = $"{tenderType}_REFUND";
            tenderRow.TerminalCode = terminalCode;
            tenderRow.TerminalDesc = terminalDesc;
            tenderRow.TransactionStartEnd = TransactionStartEndCodes.None;
            tenderRow.IsDeleted = "FALSE";

            return tenderRow;
        }
    }
}
