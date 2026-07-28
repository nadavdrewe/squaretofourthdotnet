using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Revel._808nd.com.Classes.BusinessServices;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public partial class Order : IRevelAddressable, IRevelCreateable, GrindItemSalesPeriod.ICreationDated
    {
        [Key]
        public int DBKEY_order_id { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public bool? asap { get; set; }
        public int? auto_grat_pct { get; set; }
        public int? bill_number { get; set; }
        public string bill_parent { get; set; }
        public string bills_info { get; set; }
        public int bills_type { get; set; }
        public string call_name { get; set; }
        public object call_number { get; set; }
        public bool closed { get; set; }
        public string created_at { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public bool crv_taxed { get; set; }
        public int crv_value { get; set; }
        public object customer { get; set; }
        public object delivery_clock_in { get; set; }
        public object delivery_clock_out { get; set; }
        public object delivery_employee { get; set; }
        public int dining_option { get; set; }
        public string discount { get; set; }
        public decimal discount_amount { get; set; }
        public string discount_reason { get; set; }
        public decimal? discount_rule_amount { get; set; }
        public string discount_rule_type { get; set; }
        public decimal discount_tax_amount { get; set; }
        public string discount_taxed { get; set; }
        public string establishment { get; set; }
        public bool exchange_discount { get; set; }
        public bool exchanged { get; set; }
        public object external_sync { get; set; }
        public decimal final_total { get; set; }
        public string gift_reward_data { get; set; }
        public decimal? gratuity { get; set; }
        public int gratuity_type { get; set; }
        public bool has_delivery_info { get; set; }

        public string is_discounted { get; set; }

        public string is_unpaid { get; set; }



        public int? order_id { get; set; }
        public string local_id { get; set; }
        public string notes { get; set; }
        public bool notification_email_sent { get; set; }
        public bool notification_text_sent { get; set; }
        public int number_of_people { get; set; }
        public List<string> orderhistory { get; set; }
        public object pickup_time { get; set; }
        public Nullable<int> points_added { get; set; }
        public Nullable<int> points_redeemed { get; set; }
        public string pos_mode { get; set; }
        public decimal prevailing_surcharge { get; set; }
        public decimal prevailing_tax { get; set; }
        public bool printed { get; set; }
        public Nullable<int> remaining_due { get; set; }
        public string resource_uri { get; set; }
        public int rounding_delta { get; set; }
        public decimal service_charge { get; set; }
        public decimal subtotal { get; set; }
        public decimal surcharge { get; set; }
        public object table { get; set; }
        public object table_owner { get; set; }
        public decimal tax { get; set; }
        public string tax_country { get; set; }
        public int tax_rebate { get; set; }
        public string updated_by { get; set; }
        public Nullable<DateTime> updated_date { get; set; }
        public string uuid { get; set; }
        public bool web_order { get; set; }

        //Added by Nadav
        public int establishment_id { get; set; }

        //Added by Nadav
        public int db_brand_id { get; set; }

        public Order()
        {
            theAddress = "/resources/Order?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";
        }


        public Order(dynamic jsonSingleOrderObject)
        {

            this.Create(jsonSingleOrderObject);
        }


        [JsonIgnore]
        public string theAddress { get; set; }

        public int Create(dynamic jsonSingleOrderObject)
        {
            try
            {
                theAddress = "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";
                this.order_id = Convert.ToInt32((string)jsonSingleOrderObject["id"]);
                if (this.order_id == 480914)
                {
                    var what = "";
                }
                this.discount = (string)jsonSingleOrderObject["discount"];
                try
                {
                    if ((string)jsonSingleOrderObject["discount_amount"] != null &&
                        (string)jsonSingleOrderObject["discount_amount"] != "")
                    {
                        decimal parseOut = 0.00M;
                        var ok = Decimal.TryParse((string)jsonSingleOrderObject["discount_amount"], out parseOut);
                        if (ok)
                        {
                            this.discount_amount = parseOut;
                        }

                        else
                        {
                            this.discount_amount = 0.00M;
                        }
                    }
                    else
                    {
                        this.discount_amount = 0.00M;
                    }

                }
                catch (Exception ex)
                {
                    this.discount_amount = 0.00M;
                }
                this.final_total = (string)jsonSingleOrderObject["final_total"] != "" && (string)jsonSingleOrderObject["final_total"] != null ? Convert.ToDecimal(Convert.ToDouble((string)jsonSingleOrderObject["final_total"])) : 0.00M;
                this.gratuity = (string)jsonSingleOrderObject["gratuity"] != "" && (string)jsonSingleOrderObject["gratuity"] != null ? Convert.ToDecimal((string)jsonSingleOrderObject["gratuity"]) : 0.00M;

                try
                {
                    if ((string)jsonSingleOrderObject["tax"] != null &&
                        (string)jsonSingleOrderObject["tax"] != "")
                    {
                        decimal parseOut = 0.00M;
                        var ok = Decimal.TryParse((string)jsonSingleOrderObject["tax"], out parseOut);
                        if (ok)
                        {
                            this.tax = parseOut;
                        }

                        else
                        {
                            this.tax = 0.00M;
                        }
                    }
                    else
                    {
                        this.tax = 0.00M;
                    }

                }
                catch (Exception ex)
                {
                    this.tax = 0.00M;
                }


                this.establishment = (string)jsonSingleOrderObject["establishment"];
                try
                {
                    this.establishment_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)jsonSingleOrderObject["establishment"]);
                }
                catch (Exception)
                {
                    this.establishment_id = RevelHelper.ConvertJSONEstablishmentIDFromURIToIntegerPrimaryKey((string)jsonSingleOrderObject["establishment"]);
                }
                this.closed = ((bool)jsonSingleOrderObject["closed"]);

                this.prevailing_surcharge = (string)jsonSingleOrderObject["prevailing_surcharge"] != "" && (string)jsonSingleOrderObject["prevailing_surcharge"] != null ? Convert.ToDecimal((string)jsonSingleOrderObject["prevailing_surcharge"]) : 0.00M;
                this.prevailing_tax = (string)jsonSingleOrderObject["prevailing_tax"] != "" && (string)jsonSingleOrderObject["prevailing_tax"] != null ? Convert.ToDecimal((string)jsonSingleOrderObject["prevailing_tax"]) : 0.00M;

                var subtotal = (string)jsonSingleOrderObject["subtotal"];

                /*this.subtotal = (string)jsonSingleOrderObject["subtotal"] != "" && (string)jsonSingleOrderObject["subtotal"] != null ? Convert.ToDecimal((string)jsonSingleOrderObject["subtotal"]) : 0.00M;*/
                this.surcharge = (string)jsonSingleOrderObject["surcharge"] != "" && (string)jsonSingleOrderObject["surcharge"] != null ? Convert.ToDecimal((string)jsonSingleOrderObject["surcharge"]) : 0.00M;
                this.service_charge = (string)jsonSingleOrderObject["service_charge"] != "" && (string)jsonSingleOrderObject["service_charge"] != null ? Convert.ToDecimal((string)jsonSingleOrderObject["service_charge"]) : 0.00M;

                if (service_charge > 0)
                {
                    var weGotrone = "";
                }

                string date = jsonSingleOrderObject["created_date"].ToString("yyyy-MM-dd HH:mm:ss");
                this.created_date = Convert.ToDateTime(date);


                //added to parse list 05/07/2014 by Nadav Drewe

                this.created_by = (string)jsonSingleOrderObject["created_by"];
                this.discount_reason = (string)jsonSingleOrderObject["discount_reason"];
                this.discount_rule_amount = (string)jsonSingleOrderObject["discount_rule_amount"] != null ? Convert.ToDecimal((string)jsonSingleOrderObject["discount_rule_amount"]) : 0.00M;
                this.discount_rule_type = (string)jsonSingleOrderObject["discount_rule_type"];
                this.discount_tax_amount = (string)jsonSingleOrderObject["discount_tax_amount"] != null ? Convert.ToDecimal((string)jsonSingleOrderObject["discount_tax_amount"]) : 0.00M;
                this.discount_taxed = (string)jsonSingleOrderObject["discount_taxed"];
                this.is_discounted = (string)jsonSingleOrderObject["is_discounted"];
                this.is_unpaid = (string)jsonSingleOrderObject["is_unpaid"] != null ? (string)jsonSingleOrderObject["is_unpaid"] : "";
                this.web_order = (bool)jsonSingleOrderObject["web_order"];
                this.bill_parent = (string)jsonSingleOrderObject["bill_parent"];
                this.resource_uri = jsonSingleOrderObject["resource_uri"];
                //added nd 22/10/2014

                try
                {
                    if ((string)jsonSingleOrderObject["points_added"] != null &&
                             (string)jsonSingleOrderObject["points_added"] != "")
                    {
                        points_added = (int)jsonSingleOrderObject["points_added"];
                    }


                }
                catch (Exception ex)
                {

                    throw ex;
                }


                try
                {
                    if ((string)jsonSingleOrderObject["points_redeemed"] != null &&
                                (string)jsonSingleOrderObject["points_redeemed"] != "")
                    {
                        points_redeemed = (int)jsonSingleOrderObject["points_redeemed"];
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }



                try
                {
                    if ((string)jsonSingleOrderObject["remaining_due"] != null &&
                                (string)jsonSingleOrderObject["remaining_due"] != "")
                    {
                        remaining_due = (int)jsonSingleOrderObject["remaining_due"];
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }

                //added 24/11/2014


                try
                {
                    if ((string)jsonSingleOrderObject["updated_date"] != null &&
                             (string)jsonSingleOrderObject["updated_date"] != "")
                    {
                        updated_date = Convert.ToDateTime(jsonSingleOrderObject["updated_date"].ToString("yyyy-MM-dd HH:mm:ss"));
                    }


                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }

            return 0;
        }

        public DateTime CreationDate
        {
            get { return created_date; }

        }
    }
}