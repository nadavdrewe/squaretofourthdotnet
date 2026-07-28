using com.fourth.pipeline.pos.Model;
using Square;
using System;
using System.Collections.Generic;
using System.Text;
using square.pipeline.fourth.com.Extensions;
using com.fourth.pipeline.pos.Extensions;
using System.Linq;
using com.fourth.pipeline.pos;
using domain.pipeline.fourth.com.Extensions;

namespace domain.pipeline.fourth.com.SalesFactories.SalesRowFactories
{

    /// <summary>
    /// This creates a 'base row' with static values
    /// </summary>
    public static class BaseSquareRowFactory
    {
        public static TransactionDatasetRow Create(TeamMember employee,
            Order squareOrder,
            string unitId,
            string siteLocationCode,
            string recordActivityCode
            )
        {
            //converstions
            var effectiveTimestamp = squareOrder.ClosedAt ?? squareOrder.CreatedAt ?? DateTimeOffset.UtcNow.ToString("O");
            var dateTimeUTC = DateTimeOffset.Parse(effectiveTimestamp);
            var dateUTC = dateTimeUTC.ToString("yyyy-MM-dd");
            var timeUTC = dateTimeUTC.ToFourthSalesCSVTimeUTC();


            var tableOwner = "";
            var tableownerDesc = "";
            if (employee != null)
            {
                tableOwner = employee.Id;
                tableownerDesc = employee.GivenName + " " + employee.FamilyName;
            }

            //convert to local timezone?
            return new TransactionDatasetRow
            {
                TransactionId = squareOrder.Id.ToCodedTransactionId(recordActivityCode),
                UnitId = unitId,
                SiteLocationCode = siteLocationCode,
                TradingDate = dateUTC, //are these UTC?
                Time = timeUTC, //local time or UTC
                TimeFact = "0",
                RecordActivityCode = recordActivityCode,
                ReceiptCode = squareOrder.Id,
                CheckCode = squareOrder.Id,
                //set emloyee in base row

                //set defaults cos dodn't exist in square
                RevenueCentreCode = "1",
                RevenueCentreDesc = "Default Square Revenue Center",
                //sales item
                SalesItemId = "",
                SalesItemDesc = "",
                SalesItemGUID = "",
                //table defaults,
                Covers = 1,
                TabOwner = tableOwner,
                TabOwnerDesc = tableownerDesc,
                TableCode = "",
                OriginalTabOwner = tableOwner,
                OriginalTabOwnerDesc = tableownerDesc,
                //tender defaults
                TenderTypeCode = "",
                TenderAmount = 0.00M,
                //terminal
                TerminalCode = "",
                TerminalDesc = "",
                //deductions defaults
                DeductionCode = "",
                DeductionDesc = "",
                Deduction = 0.00M,
                DeductionConv = 0.00M,
                //Trans start end default
                TransactionStartEnd = TransactionStartEndCodes.None,

                Currency = "",

                //food groups - all set to category as nothing else
                FamilyGroupDesc = "",
                CustomFact1 = "",
                CustomFact2 = "",
                CustomField1 = "",
                CustomField2 = "",
                CustomField3 = "",

                //blank always unused defaults
                OldTableCode = "",
                PrevTransactionCode = "",
                AuthorisedBy = "",
                TextField = "",
                GuestDesc = "",
                GuestCode = "",
                TimeSentToPrep = "",
                BumpTime = "",
                UniversalTimeSlotId = "",
                DateFact = "",
                IsDeleted = "FALSE",
                TimeSlotDesc = "",


                //costs
                CostPriceTheo = 0,
                CostPriceTheoConv = 0.00M,

                ListPrice = 0.00M,
                ListPriceConv = 0.00M,

                MenuBand = "",
                MajorGroupDesc = "",
                OrderTypeDesc = "TAKE AWAY", //set as 'TAKE AWAY' - there's no attribute supplied in API

                PricePaid = 0.00M,
                Qty = "0",

                TransactionTypeCode = "",

                Tax = 0.00M,
                TaxConv = 0.00M,

                PricePaidConv = 0.00M,
                SalesItemPLU = "",
                SubGroupDesc = "",
                TenderAmountConv = 0.00M,
                TenderTypeDesc = ""
            };
        }
    }
}
