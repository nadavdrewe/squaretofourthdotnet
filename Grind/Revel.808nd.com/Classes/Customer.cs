using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public  class Customer : IRevelAddressable, IRevelCreateable, IRevelDeletable/*, ICustomer*/
    {
        [JsonIgnore]
        [Key]
        public int DBKEY_customer_id { get; set; }


        //[JsonProperty("account_balance")]
        //public Nullable<decimal> AccountBalance { get; set; }

        //[JsonProperty("account_limit")]
        //public Nullable<decimal> AccountLimit { get; set; }

        [JsonProperty("active")]
        public Nullable<bool> Active { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("addresses")]
        public List<Address> Addresses { get; set; }

        [JsonProperty("birth_date")]
        public Nullable<DateTime> BirthDate { get; set; }

        [JsonProperty("cc_exp")]
        public string CcExp { get; set; }

        [JsonProperty("cc_first_name")]
        public string CcFirstName { get; set; }

        [JsonProperty("cc_last_4_digits")]
        public string CcLast4Digits { get; set; }

        [JsonProperty("cc_last_name")]
        public string CcLastName { get; set; }

        [JsonProperty("cc_number")]
        public string CcNumber { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("created_by")]
        public string CreatedBy { get; set; }

        [JsonProperty("created_date")]
        public Nullable<DateTime> CreatedDate { get; set; }

        [JsonProperty("customer_groups")]
        public List<string> CustomerGroups { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("exp_date")]
        public Nullable<DateTime> ExpDate { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("id")]
        public int RevelId { get; set; }

        /* [JsonProperty("image")]
         public object Image { get; set; }*/

        [JsonProperty("is_visitor")]
        public bool IsVisitor { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("lic_number")]
        public string LicNumber { get; set; }

        [JsonProperty("loyalty_number")]
        public string LoyaltyNumber { get; set; }

        [JsonProperty("loyalty_ref_id")]
        public string LoyaltyRefId { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("ref_number")]
        public string RefNumber { get; set; }

        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("total_purchases")]
        public int TotalPurchases { get; set; }

        [JsonProperty("total_visits")]
        public int TotalVisits { get; set; }

        [JsonProperty("updated_by")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updated_date")]
        public Nullable<DateTime> UpdatedDate { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("zipcode")]
        public string Zipcode { get; set; }



        //added by me
        [JsonIgnore]
        public int customer_id { get; set; }
        [JsonIgnore]
        public int establishment_id { get; set; }

        [JsonIgnore]
        public string theAddress { get; set; }


        //[NotMapped]
        //[JsonIgnore]
        //public RewardsCardNew RewardsCardNew { get; set; }


        public Customer()
        {
            theAddress = "/resources/Customer?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";

            Addresses = new List<Address>();
            CustomerGroups = new List<string>();
        }

        public Customer(dynamic jsonCustomer)
            : this()
        {


        }


        public int Create(dynamic jsonCustomer)
        {

            try
            {
                RevelId = jsonCustomer["id"];

                if (RevelId == 9)
                {
                    var itFaizan = true;

                }

                Active = (bool)jsonCustomer["active"];


                try
                {
                    if (jsonCustomer["birth_date"].ToString() != "" && jsonCustomer["birth_date"].ToString() != null)
                    {
                        BirthDate = Convert.ToDateTime(jsonCustomer["birth_date"].ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                }
                catch (Exception ex)
                {

                    //ignore it, whatever
                }

                CcExp = jsonCustomer["cc_exp"];
                CcFirstName = jsonCustomer["cc_first_name"];
                CcLast4Digits = jsonCustomer["cc_last_4_digits"];
                CcLastName = jsonCustomer["cc_last_name"];
                CcNumber = jsonCustomer["cc_number"];
                City = jsonCustomer["city"];
                CreatedBy = jsonCustomer["created_by"];

                try
                {
                    if (jsonCustomer["created_date"].ToString() != "" && jsonCustomer["created_date"].ToString() != null)
                    {
                        CreatedDate = Convert.ToDateTime(jsonCustomer["created_date"].ToString("yyyy-MM-dd hh:mm:ss"));
                    }

                }
                catch (Exception ex)
                {


                }
                Email = jsonCustomer["email"];


                if (jsonCustomer["exp_date"].ToString() != "" && jsonCustomer["exp_date"].ToString() != null)
                {
                    ExpDate = Convert.ToDateTime(jsonCustomer["exp_date"].ToString("yyyy-MM-dd hh:mm:ss"));
                }

                FirstName = jsonCustomer["first_name"];



                IsVisitor = Convert.ToBoolean(jsonCustomer["is_visitor"].ToString());
                LastName = jsonCustomer["last_name"];
                LicNumber = jsonCustomer["lic_number"];
                LoyaltyNumber = jsonCustomer["loyalty_number"];
                LoyaltyRefId = jsonCustomer["loyalty_ref_id"];
                Notes = jsonCustomer["notes"];

                PhoneNumber = jsonCustomer["phone_number"];
                Picture = jsonCustomer["picture"];
                RefNumber = jsonCustomer["ref_number"];
                ResourceUri = jsonCustomer["resource_uri"];
                State = jsonCustomer["state"];
                TotalPurchases = Convert.ToInt32(jsonCustomer["total_purchases"].ToString());
                TotalVisits = Convert.ToInt32(jsonCustomer["total_visits"].ToString());
                UpdatedBy = jsonCustomer["updated_by"];


                try
                {
                    if (jsonCustomer["updated_date"].ToString() != "" && jsonCustomer["updated_date"].ToString() != null)
                    {
                        UpdatedDate = Convert.ToDateTime(jsonCustomer["updated_date"].ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }


                Uuid = jsonCustomer["uuid"];
                Zipcode = jsonCustomer["zipcode"];
            }
            catch (Exception ex)
            {
                var id = RevelId;
                throw;
            }

            //the address
            try
            {
                var addresses = jsonCustomer["addresses"].ToString();
                var addressArray = JsonConvert.DeserializeObject<List<Address>>(addresses);

                //assign the 
                foreach (Address address in addressArray)
                {
                    address.customer_id = RevelId;
                    Addresses.Add(address);
                }
            }
            catch (Exception)
            {
                var id = RevelId;
                throw;
            }


            return 0;
        }

        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }

    }




}
