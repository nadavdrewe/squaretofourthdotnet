using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    using System;
    using System.Collections.Generic;

    public class OrderV2
    {
        public int RunningTaxNumber { get; set; }
        public bool WebOrder { get; set; }
        public bool HasItems { get; set; }
        public object RegistryData { get; set; }
        public object ExchangeDiscount { get; set; }
        public string GiftRewardData { get; set; }
        public string Uuid { get; set; }
        public string CreatedBy { get; set; }
        public string ServiceFeeUntaxed { get; set; }
        public object DiscountTaxAmountIncluded { get; set; }
        public object DiscountAmount { get; set; }
        public object FleetServiceData { get; set; }
        public bool Sent { get; set; }
        public object Exchanged { get; set; }
        public string UpdatedBy { get; set; }
        public string CheckSum { get; set; }
        public object DeliveryEmployee { get; set; }
        public double CrvValue { get; set; }
        public object DeliveryDuration { get; set; }
        public object TableOwner { get; set; }
        public double SurchargeExcluded { get; set; }
        public double TaxableSurchargeExcluded { get; set; }
        public int NumberOfPeople { get; set; }
        public string Notes { get; set; }
        public List<object> AppliedDiscounts { get; set; }
        public string PosMode { get; set; }
        public bool HaApplied { get; set; }
        public string DeletedDiscounts { get; set; }
        public int PointsAdded { get; set; }
        public bool SmartOrder { get; set; }
        public object BillsInfo { get; set; }
        public double TaxExcludedAmount { get; set; }
        public double AutoGratPct { get; set; }
        public bool IsDiscounted { get; set; }
        public int TaxRoundingModel { get; set; }
        public bool NotificationEmailSent { get; set; }
        public object DeliveryDistance { get; set; }
        public double Gratuity { get; set; }
        public List<string> OrderHistory { get; set; }
        public object LoyaltyAccountId { get; set; }
        public int Version { get; set; }
        public bool Closed { get; set; }
        public string TaxCountry { get; set; }
        public object DiscountCode { get; set; }
        public double SmartpayTip { get; set; }
        public bool HasDeliveryInfo { get; set; }
        public bool Asap { get; set; }
        public bool Deleted { get; set; }
        public object PickupTime { get; set; }
        public object ReportingId { get; set; }
        public object DiscountNontaxableSurchargeIncluded { get; set; }
        public double TaxableSurcharge { get; set; }
        public object DeliveryAddress { get; set; }
        public double FinalTotal { get; set; }
        public List<object> Package { get; set; }
        public bool IsInvoice { get; set; }
        public object DeliveryClockIn { get; set; }
        public string CreatedDate { get; set; }
        public object DeliveryClockOut { get; set; }
        public object DiscountTaxAmount { get; set; }
        public double RoundingDelta { get; set; }
        public object DriveThroughData { get; set; }
        public bool IsUnpaid { get; set; }
        public object CustomerAddressDistance { get; set; }
        public double TaxRebate { get; set; }
        public string Table { get; set; }
        public string DiscountReason { get; set; }
        public bool NotificationTextSent { get; set; }
        public string LastUpdatedAt { get; set; }
        public object CustomerBirthdate { get; set; }
        public int GratuityType { get; set; }
        public object Vehicle { get; set; }
        public double RemainingDue { get; set; }
        public List<AppliedTaxOrder> AppliedTaxes { get; set; }
        public double PrevailingTax { get; set; }
        public object BillingAddress { get; set; }
        public object InvoiceDate { get; set; }
        public object DiscountRuleType { get; set; }
        public int KitchenStatus { get; set; }
        public double ServiceCharge { get; set; }
        public string Customer { get; set; }
        public List<object> AppliedServiceFee { get; set; }
        public Dictionary<string, object> VirtualData { get; set; }
        public object DiscountTaxed { get; set; }
        public bool HasHistory { get; set; }
        public object BillingZipCode { get; set; }
        public double SmartpayGratuity { get; set; }
        public double Tax { get; set; }
        public int BillNumber { get; set; }
        public int Id { get; set; }
        public double Surcharge { get; set; }
        public int PointsRedeemed { get; set; }
        public object CallNumber { get; set; }
        public object DeliveryEstimatedDistance { get; set; }
        public object PickupData { get; set; }
        public object BillParent { get; set; }
        public object Discount { get; set; }
        public string Establishment { get; set; }
        public string UpdatedDate { get; set; }
        public double PrevailingSurcharge { get; set; }
        public int DiningOption { get; set; }
        public int BillsType { get; set; }
        public object CallName { get; set; }
        public bool Printed { get; set; }
        public double Subtotal { get; set; }
        public object DeviceId { get; set; }
        public object DiscountedBy { get; set; }
        public object DiscountRuleAmount { get; set; }
        public bool IsReadonly { get; set; }
        public object ExternalSync { get; set; }
        public string CreatedAt { get; set; }
        public string ServiceFeeTax { get; set; }
        public string DiscountTotalAmount { get; set; }
        public bool CrvTaxed { get; set; }
        public string LocalId { get; set; }
        public string ServiceFeeTaxed { get; set; }
        public string ResourceUri { get; set; }
    }

    public class AppliedTaxOrder
    {
        public int MaxQuantityThreshold { get; set; }
        public int MinQuantityThreshold { get; set; }
        public string FiscalRate { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string MaxThreshold { get; set; }
        public string MinThreshold { get; set; }
        public int RoundingType { get; set; }
        public bool OnFullPrice { get; set; }
        public string Order { get; set; }
        public bool IsPrevailing { get; set; }
        public string TaxRate { get; set; }
        public object TaxTable { get; set; }
        public int Id { get; set; }
        public int LocalTaxId { get; set; }
        public int TaxType { get; set; }
        public string DiningOptions { get; set; }
        public string ResourceUri { get; set; }
    }

}
