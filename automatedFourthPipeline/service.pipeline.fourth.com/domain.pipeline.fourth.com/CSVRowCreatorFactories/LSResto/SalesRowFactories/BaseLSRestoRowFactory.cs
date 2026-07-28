using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Extensions;
using com.fourth.pipeline.pos.Model;
using core.lightspeed.com.Models.Core;
using core.lightspeed.com.Models.Financial.Receipts;
using core.lightspeed.com.Models.Labour;
using domain.pipeline.fourth.com.Extensions;
using domain.pipeline.fourth.com.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories
{
    public static class BaseLSRestoRowFactory
    {
        public static TransactionDatasetRow Create(
            Employee employee,
            Receipt receipt,
            Reservation reservation,
            DateTime transactionDate,
            string unitId,
            string siteLocationCode,
            string recordActivityCode,
            string deviceId = "",
            string deviceDesc = "")
        {
            //converstions        

            //the tran date HAS TO BE DAY BEFORE IF AFTER MIDNIGHT            
            var dateTiimeUTC = transactionDate;            
            var dateUTC = dateTiimeUTC.ToString("yyyy-MM-dd");
            var timeUTC = Convert.ToDateTime(receipt.creationDate).ToFourthSalesCSVTimeUTC();

            var tableOwner = employee.userId.ToString();
            var tableownerDesc = employee.username;




            ////convert to local timezone?            
            return new TransactionDatasetRow
            {
                TransactionId = receipt.id.ToString().ToCodedTransactionId(recordActivityCode),
                UnitId = unitId,
                SiteLocationCode = siteLocationCode,
                TradingDate = dateUTC, //are these UTC?
                Time = timeUTC, //local time or UTC  
                TimeFact = "0",
                RecordActivityCode = recordActivityCode,
                ReceiptCode = receipt.id.ToString(),
                CheckCode = receipt.id.ToString(),
                //set emloyee in base row

                //set defaults cos dodn't exist in square                
                RevenueCentreCode = "1",
                RevenueCentreDesc = "Default LS Restaurant Revenue Center",
                //sales item
                SalesItemId = "",
                SalesItemDesc = "",
                SalesItemGUID = "",
                //table defaults,
                Covers = reservation?.seats ?? 1,
                TabOwner = tableOwner,
                TabOwnerDesc = tableownerDesc,
                TableCode = receipt.tableId.ToString(),
                OriginalTabOwner = tableOwner,
                OriginalTabOwnerDesc = tableownerDesc,
                //tender defaults
                TenderTypeCode = "",
                TenderAmount = 0.00M,
                //terminal
                TerminalCode = deviceId,
                TerminalDesc = deviceDesc,
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
                OrderTypeDesc = receipt.type, //this is 'type' in receipt

                PricePaid = 0.00M,
                Qty = "0",

                TransactionTypeCode = "",

                Tax = 0.00M,
                TaxConv = 0.00M,

                PricePaidConv = 0.00M,
                SalesItemPLU = "",
                SubGroupDesc = "",
                TenderAmountConv = 0.00M,
                TenderTypeDesc = "",
            };

        }
    }
}
