using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using domain.pipeline.fourth.com.Helper;
using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using Square;
using System;

namespace domain.pipeline.fourth.com.Square.SalesFactories.SalesRowFactories.ServiceChargeRow
{
    public static class ServiceChargeRowFactory
    {
        public static TransactionDatasetRow CreateForOrderServiceCharge(
            TeamMember employee,
            Order squareOrder,
            OrderServiceCharge serviceCharge,
            string unitId,
            string siteLocationCode,
            string newRecordActivityCode,
            string terminalCode,
            string terminalDesc)
        {
            var amountMoney = serviceCharge.TotalMoney ?? serviceCharge.AppliedMoney ?? serviceCharge.AmountMoney;
            var taxMoney = serviceCharge.TotalTaxMoney;
            return Create(
                employee,
                squareOrder,
                serviceCharge.Uid,
                serviceCharge.CatalogObjectId,
                serviceCharge.Name,
                amountMoney,
                taxMoney,
                unitId,
                siteLocationCode,
                newRecordActivityCode,
                terminalCode,
                terminalDesc);
        }

        public static TransactionDatasetRow CreateForTenderTip(
            TeamMember employee,
            Order squareOrder,
            Tender tender,
            Payment payment,
            string unitId,
            string siteLocationCode,
            string newRecordActivityCode,
            string terminalCode,
            string terminalDesc)
        {
            var tipMoney = tender.TipMoney ?? payment?.TipMoney;
            return Create(
                employee,
                squareOrder,
                tender.PaymentId,
                "TIP",
                "Tip",
                tipMoney,
                null,
                unitId,
                siteLocationCode,
                newRecordActivityCode,
                terminalCode,
                terminalDesc);
        }

        private static TransactionDatasetRow Create(
            TeamMember employee,
            Order squareOrder,
            string sourceId,
            string plu,
            string description,
            Money amountMoney,
            Money taxMoney,
            string unitId,
            string siteLocationCode,
            string newRecordActivityCode,
            string terminalCode,
            string terminalDesc)
        {
            var serviceChargeRow = BaseSquareRowFactory.Create(
                employee,
                squareOrder,
                unitId,
                siteLocationCode,
                newRecordActivityCode);

            var amount = Convert.ToDecimal(amountMoney?.Amount ?? 0).DivideBy100();
            var tax = Convert.ToDecimal(taxMoney?.Amount ?? 0).DivideBy100();

            serviceChargeRow.TerminalCode = terminalCode;
            serviceChargeRow.TerminalDesc = terminalDesc;
            serviceChargeRow.TransactionTypeCode = TransactionTypeCodes.SERVICE_CHARGE;
            serviceChargeRow.SalesItemId = sourceId ?? "";
            serviceChargeRow.SalesItemPLU = plu ?? "";
            serviceChargeRow.SalesItemGUID = sourceId ?? "";
            serviceChargeRow.SalesItemDesc = string.IsNullOrWhiteSpace(description) ? "Service Charge" : description;
            serviceChargeRow.MajorGroupDesc = "Hospitality";
            serviceChargeRow.FamilyGroupDesc = "Hospitality";
            serviceChargeRow.SubGroupDesc = "Hospitality";
            serviceChargeRow.Qty = "1";
            serviceChargeRow.ListPrice = amount;
            serviceChargeRow.Tax = tax;
            serviceChargeRow.Currency = amountMoney?.Currency?.ToString() ?? "";
            serviceChargeRow.PricePaid = amount;
            serviceChargeRow.Deduction = 0.00M;
            serviceChargeRow.ListPriceConv = amount;
            serviceChargeRow.TaxConv = tax;
            serviceChargeRow.PricePaidConv = amount;
            serviceChargeRow.DeductionConv = 0.00M;
            serviceChargeRow.CostPriceTheo = 0.00M;
            serviceChargeRow.CostPriceTheoConv = 0.00M;
            serviceChargeRow.TransactionStartEnd = TransactionStartEndCodes.None;
            serviceChargeRow.IsDeleted = "FALSE";

            return serviceChargeRow;
        }
    }
}
