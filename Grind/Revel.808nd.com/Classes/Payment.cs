
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public partial class Payment : IRevelAddressable, IRevelCreateable
    {
        [NotMapped]
        public string theAddress { get; set; }

        [Key]
        public int DBKEY_payment_id { get; set; }
        public decimal amount { get; set; }
        public Nullable<decimal> amount_authorized { get; set; }
        /*    public int bill { get; set; }*/
        public string card_type { get; set; }
        public string cc_first_name { get; set; }
        public string cc_last_name { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> created_date { get; set; }

        public Nullable<bool> deleted { get; set; }
        public string establishment { get; set; }
        /*public object exchanged { get; set; }*/
        public bool executed { get; set; }
        public string first_4_cc_digits { get; set; }
        /*public int gratuity { get; set; }*/
        public int id { get; set; }
        public string last_4_cc_digits { get; set; }
        public string order { get; set; }
        public string other_payment_type { get; set; }

        /*public object payer_id { get; set; }*/
        public Nullable<DateTime> payment_date { get; set; }
        public Nullable<int> payment_type { get; set; }

        /*
        public bool processor_accepted { get; set; }
        public object processor_response { get; set; }
        public object receipt_email { get; set; }
         * */
        public string refund_transaction_id { get; set; }
        /*public string resource_uri { get; set; }
        public int rounding_delta { get; set; }
        public string signature_img_url { get; set; }
        public string station { get; set; }
        public int tip { get; set; }
        public bool transaction_captured { get; set; }
        public string transaction_data { get; set; }
        public string transaction_id { get; set; }
        public string transaction_status { get; set; }
        public string updated_by { get; set; }*/
        public Nullable<DateTime> updated_date { get; set; }
        /*public string uuid { get; set; }*/



        //added ND
        public Nullable<int> order_id { get; set; }
        public Nullable<int> establishment_id { get; set; }


        public Payment()
        {
           theAddress = "/resources/Payment?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";
        }

        public Payment(dynamic jsonSinglePaymentObject) 
        {
            theAddress = "/resources/Payment?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";

            try
            {
                this.amount = Convert.ToDecimal((string)jsonSinglePaymentObject["amount"]);
            }
            catch (Exception ex1)
            {
                try
                {
                    amount = decimal.Parse((string)jsonSinglePaymentObject["amount"], NumberStyles.Any);
                }
                catch (Exception ex2)
                {
                    this.amount = 0.00M;
                }
            }
            //this.amount_authorized = Convert.ToDecimal((string)jsonSinglePaymentObject["amount_authorized"]);

            card_type = (string)jsonSinglePaymentObject["card_type"];

            try
            {
                string createddate = jsonSinglePaymentObject["created_date"].ToString("yyyy-MM-dd HH:mm:ss");
                created_date = DateTime.Parse(createddate);

            }
            catch (Exception ex)
            {

                throw;
            }

            try
            {
                updated_date = (string)jsonSinglePaymentObject["updated_date"] != "" && (string)jsonSinglePaymentObject["updated_date"] != null ? Convert.ToDateTime((string)jsonSinglePaymentObject["updated_date"].ToString("yyyy-MM-dd HH:mm:ss"))
                   : new DateTime(1901, 01, 01);
            }
            catch (Exception ex)
            {

                throw;
            }

         

            try
            {
                payment_date = (string)jsonSinglePaymentObject["payment_date"] != "" && (string)jsonSinglePaymentObject["payment_date"] != null ? Convert.ToDateTime((string)jsonSinglePaymentObject["payment_date"].ToString("yyyy-MM-dd HH:mm:ss"))
                   : new DateTime(1901, 01, 01);
            }
            catch (Exception ex)
            {

                throw;
            }




            deleted = Convert.ToBoolean((string)jsonSinglePaymentObject["deleted"]);

            this.id = Convert.ToInt32((string)jsonSinglePaymentObject["id"]);
            this.establishment = (string)jsonSinglePaymentObject["establishment"];

            try
            {
                this.establishment_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)jsonSinglePaymentObject["establishment"]);
            }
            catch (Exception ex1)
            {
                try
                {
                    this.establishment_id = RevelHelper.ConvertParentOrderIDToIntegerPrimaryKey((string)jsonSinglePaymentObject["establishment"]);
                }
                catch (Exception ex2)
                {

                    try
                    {
                        this.establishment_id = RevelHelper.ConvertSpecialistIDWithSpacesToID((string)jsonSinglePaymentObject["establishment"]);
                    }
                    catch (Exception ex3)
                    {
                        //stumped
                        throw ex3;
                    }
                }
            }

            this.order = (string)jsonSinglePaymentObject["order"];
            try
            {
                this.order_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)jsonSinglePaymentObject["order"]);
            }
            catch (Exception ex1)
            {

                try
                {
                    this.order_id = RevelHelper.ConvertParentOrderIDToIntegerPrimaryKey((string)jsonSinglePaymentObject["order"]);
                }
                catch (Exception ex2)
                {
                    try
                    {
                        this.order_id = RevelHelper.ConvertSpecialistIDWithSpacesToID((string)jsonSinglePaymentObject["order"]);
                    }
                    catch (Exception ex)
                    {
                        //fuck knows
                        throw ex;
                    }

                }
            }


            //new added 22/10/2014 ND

            try
            {
                cc_first_name = (string)jsonSinglePaymentObject["cc_first_name"];
                cc_last_name = (string)jsonSinglePaymentObject["cc_last_name"];
                first_4_cc_digits = (string)jsonSinglePaymentObject["first_4_cc_digits"];
                last_4_cc_digits = (string)jsonSinglePaymentObject["last_4_cc_digits"];
                card_type = (string)jsonSinglePaymentObject["card_type"];
                other_payment_type = (string)jsonSinglePaymentObject["other_payment_type"];

            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        public int Create(dynamic jsonSinglePaymentObject)
        {
            theAddress = "/resources/Payment?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";

            try
            {
                this.amount = Convert.ToDecimal((string)jsonSinglePaymentObject["amount"]);
            }
            catch (Exception ex1)
            {
                try
                {
                    amount = decimal.Parse((string)jsonSinglePaymentObject["amount"], NumberStyles.Any);
                }
                catch (Exception ex2)
                {
                    this.amount = 0.00M;
                }
            }
            //this.amount_authorized = Convert.ToDecimal((string)jsonSinglePaymentObject["amount_authorized"]);

            card_type = (string)jsonSinglePaymentObject["card_type"];

            try
            {
                string createddate = jsonSinglePaymentObject["created_date"].ToString("yyyy-MM-dd HH:mm:ss");
                created_date = DateTime.Parse(createddate);

            }
            catch (Exception ex)
            {

                throw;
            }

            try
            {
                updated_date = (string)jsonSinglePaymentObject["updated_date"] != "" && (string)jsonSinglePaymentObject["updated_date"] != null ? Convert.ToDateTime((string)jsonSinglePaymentObject["updated_date"].ToString("yyyy-MM-dd HH:mm:ss"))
                   : new DateTime(1901, 01, 01);
            }
            catch (Exception ex)
            {

                throw;
            }



            try
            {
                payment_date = (string)jsonSinglePaymentObject["payment_date"] != "" && (string)jsonSinglePaymentObject["payment_date"] != null ? Convert.ToDateTime((string)jsonSinglePaymentObject["payment_date"].ToString("yyyy-MM-dd HH:mm:ss"))
                   : new DateTime(1901, 01, 01);
            }
            catch (Exception ex)
            {

                throw;
            }




            deleted = Convert.ToBoolean((string)jsonSinglePaymentObject["deleted"]);

            this.id = Convert.ToInt32((string)jsonSinglePaymentObject["id"]);
            this.establishment = (string)jsonSinglePaymentObject["establishment"];

            try
            {
                this.establishment_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)jsonSinglePaymentObject["establishment"]);
            }
            catch (Exception ex1)
            {
                try
                {
                    this.establishment_id = RevelHelper.ConvertParentOrderIDToIntegerPrimaryKey((string)jsonSinglePaymentObject["establishment"]);
                }
                catch (Exception ex2)
                {

                    try
                    {
                        this.establishment_id = RevelHelper.ConvertSpecialistIDWithSpacesToID((string)jsonSinglePaymentObject["establishment"]);
                    }
                    catch (Exception ex3)
                    {
                        //stumped
                        throw ex3;
                    }
                }
            }

            this.order = (string)jsonSinglePaymentObject["order"];
            try
            {
                this.order_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)jsonSinglePaymentObject["order"]);
            }
            catch (Exception ex1)
            {

                try
                {
                    this.order_id = RevelHelper.ConvertParentOrderIDToIntegerPrimaryKey((string)jsonSinglePaymentObject["order"]);
                }
                catch (Exception ex2)
                {
                    try
                    {
                        this.order_id = RevelHelper.ConvertSpecialistIDWithSpacesToID((string)jsonSinglePaymentObject["order"]);
                    }
                    catch (Exception ex)
                    {
                        //fuck knows
                        throw ex;
                    }

                }
            }


            //new added 22/10/2014 ND

            try
            {
                cc_first_name = (string)jsonSinglePaymentObject["cc_first_name"];
                cc_last_name = (string)jsonSinglePaymentObject["cc_last_name"];
                first_4_cc_digits = (string)jsonSinglePaymentObject["first_4_cc_digits"];
                last_4_cc_digits = (string)jsonSinglePaymentObject["last_4_cc_digits"];
                card_type = (string)jsonSinglePaymentObject["card_type"];
                other_payment_type = (string)jsonSinglePaymentObject["other_payment_type"];

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return 0;
        }


    }



}

