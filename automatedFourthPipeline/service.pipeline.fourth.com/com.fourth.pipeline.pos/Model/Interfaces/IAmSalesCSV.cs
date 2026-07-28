using System;
using System.Collections.Generic;
using System.Text;

namespace com.fourth.pipeline.pos.Model
{
    /// <summary>
    /// This is the format needed for the sales CSV
    /// </summary>
    public interface IAmTransactionDatasetRow
    {
        string TransactionId { get; set; }
        string UnitId { get; set; }

        string SiteLocationCode { get; set; }
        string TradingDate { get; set; }
        string Time { get; set; }
        string TimeFact { get; set; }
        string TerminalCode { get; set; }
        string TerminalDesc { get; set; }

        string RecordActivityCode { get; set; }
        string ReceiptCode { get; set; }
        string CheckCode { get; set; }
        string TableCode { get; set; }

        string RevenueCentreCode { get; set; }
        string RevenueCentreDesc { get; set; }

        string TransactionTypeCode { get; set; }
        string SalesItemId { get; set; }
        string SalesItemPLU { get; set; }
        string SalesItemGUID { get; set; }
        string SalesItemDesc { get; set; }
        string TenderTypeCode { get; set; }
        string TenderTypeDesc { get; set; }
        string DeductionCode { get; set; }
        string DeductionDesc { get; set; }
        int Covers { get; set; }
        string Qty { get; set; }
        string Currency { get; set; }

        decimal ListPrice { get; set; }
        decimal Tax { get; set; }
        decimal PricePaid { get; set; }
        decimal Deduction { get; set; }

        decimal TenderAmount { get; set; }
        decimal CostPriceTheo { get; set; }
        decimal ListPriceConv { get; set; }
        decimal TaxConv { get; set; }

        decimal PricePaidConv { get; set; }
        decimal DeductionConv { get; set; }
        decimal TenderAmountConv { get; set; }
        decimal CostPriceTheoConv { get; set; }

        string OrderTypeDesc { get; set; }
        string MenuBand { get; set; }
        string MajorGroupDesc { get; set; }

        string FamilyGroupDesc { get; set; }
        string SubGroupDesc { get; set; }
        string TabOwner { get; set; }
        string TabOwnerDesc { get; set; }
        string OriginalTabOwner { get; set; }
        string OriginalTabOwnerDesc { get; set; }
        string OldTableCode { get; set; }
        string PrevTransactionCode { get; set; }
        string AuthorisedBy { get; set; }
        string TextField { get; set; }

        string GuestDesc { get; set; }
        string GuestCode { get; set; }
        string TimeSentToPrep { get; set; }
        string BumpTime { get; set; }
        string UniversalTimeSlotId { get; set; }

        string TimeSlotDesc { get; set; }
        string TransactionStartEnd { get; set; }

        string IsDeleted { get; set; }
        string CustomField1 { get; set; }
        string CustomField2 { get; set; }
        string CustomField3 { get; set; }
        string CustomFact1 { get; set; }
        string CustomFact2 { get; set; }
        string DateFact { get; set; }


    }
}
