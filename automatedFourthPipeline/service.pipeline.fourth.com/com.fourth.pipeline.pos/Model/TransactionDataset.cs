using System;
using System.Collections.Generic;
using System.Text;

namespace com.fourth.pipeline.pos.Model
{
    public class TransactionDatasetRow : IAmTransactionDatasetRow
    {
        public string TransactionId { get; set; }
        public string UnitId { get; set; }
        public string SiteLocationCode { get; set; }
        public string TradingDate { get; set; }
        public string Time { get; set; }
        public string TimeFact { get; set; }
        public string TerminalCode { get; set; }
        public string TerminalDesc { get; set; }
        public string RecordActivityCode { get; set; }
        public string ReceiptCode { get; set; }
        public string CheckCode { get; set; }
        public string TableCode { get; set; }
        public string RevenueCentreCode { get; set; }
        public string RevenueCentreDesc { get; set; }
        public string TransactionTypeCode { get; set; }
        public string SalesItemId { get; set; }
        public string SalesItemPLU { get; set; }
        public string SalesItemGUID { get; set; }
        public string SalesItemDesc { get; set; }
        public string TenderTypeCode { get; set; }
        public string TenderTypeDesc { get; set; }
        public string DeductionCode { get; set; }
        public string DeductionDesc { get; set; }
        public int Covers { get; set; }
        public string Qty { get; set; }
        public string Currency { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal PricePaid { get; set; }
        public decimal Deduction { get; set; }
        public decimal TenderAmount { get; set; }
        public decimal CostPriceTheo { get; set; }
        public decimal ListPriceConv { get; set; }
        public decimal TaxConv { get; set; }
        public decimal PricePaidConv { get; set; }
        public decimal DeductionConv { get; set; }
        public decimal TenderAmountConv { get; set; }
        public decimal CostPriceTheoConv { get; set; }
        public string OrderTypeDesc { get; set; }
        public string MenuBand { get; set; }
        public string MajorGroupDesc { get; set; }
        public string FamilyGroupDesc { get; set; }
        public string SubGroupDesc { get; set; }
        public string TabOwner { get; set; }
        public string TabOwnerDesc { get; set; }
        public string OriginalTabOwner { get; set; }
        public string OriginalTabOwnerDesc { get; set; }
        public string OldTableCode { get; set; }
        public string PrevTransactionCode { get; set; }
        public string AuthorisedBy { get; set; }
        public string TextField { get; set; }
        public string GuestDesc { get; set; }
        public string GuestCode { get; set; }
        public string TimeSentToPrep { get; set; }
        public string BumpTime { get; set; }
        public string UniversalTimeSlotId { get; set; }
        public string TimeSlotDesc { get; set; }
        public string TransactionStartEnd { get; set; }
        public string IsDeleted { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomFact1 { get; set; }
        public string CustomFact2 { get; set; }
        public string DateFact { get; set; }
    }
}
