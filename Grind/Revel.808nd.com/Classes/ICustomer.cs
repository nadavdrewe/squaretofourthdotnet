using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Revel._808nd.com.Classes
{
    public interface ICustomer
    {
        [JsonIgnore]
        [Key]
        int DBKEY_customer_id { get; set; }

        [JsonProperty("active")]
        Nullable<bool> Active { get; set; }

        [JsonProperty("address")]
        string Address { get; set; }

        [JsonProperty("addresses")]
        List<Address> Addresses { get; set; }

        [JsonProperty("birth_date")]
        Nullable<DateTime> BirthDate { get; set; }

        [JsonProperty("cc_exp")]
        string CcExp { get; set; }

        [JsonProperty("cc_first_name")]
        string CcFirstName { get; set; }

        [JsonProperty("cc_last_4_digits")]
        string CcLast4Digits { get; set; }

        [JsonProperty("cc_last_name")]
        string CcLastName { get; set; }

        [JsonProperty("cc_number")]
        string CcNumber { get; set; }

        [JsonProperty("city")]
        string City { get; set; }

        [JsonProperty("created_by")]
        string CreatedBy { get; set; }

        [JsonProperty("created_date")]
        Nullable<DateTime> CreatedDate { get; set; }

        [JsonProperty("customer_groups")]
        List<string> CustomerGroups { get; set; }

        [JsonProperty("email")]
        string Email { get; set; }

        [JsonProperty("exp_date")]
        Nullable<DateTime> ExpDate { get; set; }

        [JsonProperty("first_name")]
        string FirstName { get; set; }

        [JsonProperty("id")]
        int RevelId { get; set; }

        [JsonProperty("is_visitor")]
        bool IsVisitor { get; set; }

        [JsonProperty("last_name")]
        string LastName { get; set; }

        [JsonProperty("lic_number")]
        string LicNumber { get; set; }

        [JsonProperty("loyalty_number")]
        string LoyaltyNumber { get; set; }

        [JsonProperty("loyalty_ref_id")]
        string LoyaltyRefId { get; set; }

        [JsonProperty("notes")]
        string Notes { get; set; }

        [JsonProperty("phone_number")]
        string PhoneNumber { get; set; }

        [JsonProperty("picture")]
        string Picture { get; set; }

        [JsonProperty("ref_number")]
        string RefNumber { get; set; }

        [JsonProperty("resource_uri")]
        string ResourceUri { get; set; }

        [JsonProperty("state")]
        string State { get; set; }

        [JsonProperty("total_purchases")]
        int TotalPurchases { get; set; }

        [JsonProperty("total_visits")]
        int TotalVisits { get; set; }

        [JsonProperty("updated_by")]
        string UpdatedBy { get; set; }

        [JsonProperty("updated_date")]
        Nullable<DateTime> UpdatedDate { get; set; }

        [JsonProperty("uuid")]
        string Uuid { get; set; }

        [JsonProperty("zipcode")]
        string Zipcode { get; set; }

        [JsonIgnore]
        int customer_id { get; set; }

        [JsonIgnore]
        int establishment_id { get; set; }

        [JsonIgnore]
        string theAddress { get; set; }

        [NotMapped]
        [JsonIgnore]
        RewardsCardNew RewardsCardNew { get; set; }

        int Create(dynamic jsonCustomer);
    }
}