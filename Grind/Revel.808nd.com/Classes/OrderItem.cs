using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Revel._808nd.com.Classes.BusinessServices;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;
using System.Linq.Expressions;

namespace Revel._808nd.com.Classes
{
    public partial class OrderItem : IRevelTimeable, IRevelAddressable, IRevelCreateable, IEquatable<OrderItem>, GrindItemSalesPeriod.ICreationDated, GrindItemSalesPeriod.IParentable, GrindItemSalesPeriod.IPricable
    {

        public OrderItem()
        {
            theAddress =
                "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";
        }

        public static List<OrderItem> AssignProductASKUAndEstablishmentToOrderItems(IList<Product> products, IList<OrderItem> itemsToAssignTo,
            out List<OrderItem> errorItems)
        {

            errorItems = new List<OrderItem>();
            var orderItemsToReturn = new List<OrderItem>();

            foreach (var item in itemsToAssignTo)
            {
                try
                {
                    var prod = products.Where(x => x.resource_uri == item.product).FirstOrDefault();

                    if (prod != null)
                    {
                        item.db_establishment_id = prod.db_establishment_id;
                        item.db_brand_id = prod.db_brand_id;
                        item.sku = prod.sku;
                        item.establishment = prod.establishment;
                        item.establishment_id = prod.establishment_id;
                        item.IsItemWithoutProduct = false;
                        orderItemsToReturn.Add(item);
                    }
                    else
                    {
                        item.IsItemWithoutProduct = true;
                        errorItems.Add(item);
                        //it's a special item without a product - set the tag and 
                    }

                }
                catch (Exception)
                {
                    errorItems.Add(item);
                }
            }

            return orderItemsToReturn;
        }

        public OrderItem(string URL)
        {
            theAddress = URL;
        }
        public static List<OrderItem> ReturnSpecificItemTypeBasedOnProducts(List<OrderItem> Items, List<Product> products)
        {
            List<OrderItem> justThoseItems = new List<OrderItem>();
            //get the correct items out of the item list
            foreach (var item in Items)
            {
                foreach (var prod in products)
                {
                    if (item.product_id == prod.product_id)
                    {
                        justThoseItems.Add(item);
                    }
                }
            }
            //foreach item subtract close / open

            //divide by number of items

            return justThoseItems;
        }


        /// <summary>
        /// We only map what we need!!
        /// </summary>
        /// <param name="jsonSingleOrderItemObject"></param>
        public OrderItem(dynamic jsonSingleOrderItemObject)
        {
            this.Create(jsonSingleOrderItemObject);
            theAddress =
                  "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";


        }


        [Key]
        public int DBKEY_orderitem_id { get; set; }


        public object barmaxx_status { get; set; }
        public object bill_parent { get; set; }
        public bool catering_complete { get; set; }
        public object catering_delivery_date { get; set; }
        public object combo_used { get; set; }
        public object combo_uuid { get; set; }
        public string commission { get; set; }
        public decimal cost { get; set; }
        public int course_number { get; set; }
        public string created_by { get; set; }
        public DateTime? created_date { get; set; }
        public decimal crv_value { get; set; }
        public int cup_qty { get; set; }
        public decimal cup_weight { get; set; }
        public object date_paid { get; set; }
        public bool deleted { get; set; }
        public int dining_option { get; set; }
        public string discount { get; set; } //??
        public decimal discount_amount { get; set; }
        public string discount_reason { get; set; }
        public decimal discount_rule_amount { get; set; }
        public object discount_rule_type { get; set; }
        public bool discount_taxed { get; set; }
        public bool? exchange_discount { get; set; }
        public bool? exchanged { get; set; }
        public DateTime? expedited { get; set; }

        public string ervc_type { get; set; }

        public int orderitem_id { get; set; }
        public decimal initial_price { get; set; }
        public bool is_cold { get; set; }
        public bool is_coupon { get; set; }
        public bool is_gift { get; set; }
        public DateTime? kitchen_completed { get; set; }
        public decimal modifier_amount { get; set; }
        public decimal modifier_cost { get; set; }
        public string modifieritems { get; set; } //used to be list I converted to simple type
        public bool on_hold { get; set; }
        public string order { get; set; }
        public string order_local_id { get; set; }
        public object parent_uuid { get; set; }
        public decimal price { get; set; }
        public bool printed { get; set; }
        public string product { get; set; }
        public string product_name_override { get; set; }
        public int quantity { get; set; }
        public string resource_uri { get; set; }
        public object seat_number { get; set; }
        public int shared { get; set; }
        public string special_request { get; set; }
        public int split_parts { get; set; }
        public int split_type { get; set; }
        public int split_with_seat { get; set; }
        public string station { get; set; }
        public decimal tax_amount { get; set; }
        public decimal tax_rate { get; set; }
        public int tax_rebate { get; set; }
        public bool taxed_flag { get; set; }
        public int temp_sort { get; set; }
        public string updated_by { get; set; }
        public Nullable<DateTime> updated_date { get; set; }
        public string uuid { get; set; }
        public string voided_by { get; set; }
        public DateTime? voided_date { get; set; }
        public string voided_reason { get; set; }
        public decimal weight { get; set; }


        //added variables
        public decimal total_price_after_tax { get; private set; }
        public decimal total_price_after_discount { get; private set; }
        public int parent_order_id { get; private set; }
        public int product_id { get; set; }
        public int discount_id { get; set; }
        public decimal pure_sales { get; set; }
        private decimal discountMoneyAmountACTUAL { get; set; }

        //added for 
        public string establishment { get; set; }
        public string brand { get; set; }
        public string sku { get; set; }



        public int db_product_id { get; set; }
        public int db_brand_id { get; set; }
        public int db_establishment_id { get; set; }


        //this is the revel Id
        [JsonIgnore]
        public int establishment_id { get; set; }

        public DateTime? start_time { get; set; }

        public bool? IsItemWithoutProduct { get; set; }

        public bool CalculateTotalPriceAfterTax()
        {
            if (taxed_flag == true)
            {
                total_price_after_tax = (initial_price - tax_amount);

                return true;
            }
            return false;
        }

        //added by ND



        public decimal CalculateRealDiscountMoneyAmount()
        {

            const decimal REWARDAMOUNTMULTIPLIER = 0.742222M;

            //is there a discount link? if not, times by *0.xx as it's a 'reward'
            if (discount.Equals("") || discount.ToLower().Contains("reward"))
            {

                return pure_sales * REWARDAMOUNTMULTIPLIER;

            }
            else
            {

                using (GrindContext context = new GrindContext())
                {
                    var parsedID = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(this.discount);
                    var discountMultiplier = Convert.ToDecimal(double.Parse(context.Discounts.Where(x => x.id == parsedID).FirstOrDefault().discount_amount.ToString()));

                    return pure_sales * (discountMultiplier / 100.00M);

                }

                //round it up to 2dp
                return 0;
            }


        }




        public string theAddress { get; set; }


        public int Create(dynamic jsonSingleOrderItemObject)
        {
            try
            {
                //keybase
                this.orderitem_id = Convert.ToInt32(jsonSingleOrderItemObject["id"].ToString());

                //testing for order item
                if (this.orderitem_id == 76259) //testing
                {
                    var pause = "";

                }

                this.product = (string)jsonSingleOrderItemObject["product"];
                this.order = (string)jsonSingleOrderItemObject["order"];

                this.voided_by = (string)jsonSingleOrderItemObject["voided_by"];
                //   this.voided_date = Convert.ToDateTime(jsonSingleOrderItemObject["voided_date"].ToString());
                this.voided_reason = jsonSingleOrderItemObject["voided_reason"].ToString();
                //   this.modifieritems = jsonSingleOrderItemObject["modifieritems"];
                //end relationships

                this.created_date = Convert.ToDateTime(jsonSingleOrderItemObject["created_date"].ToString("yyyy-MM-dd HH:mm:ss"));

                if (jsonSingleOrderItemObject["cost"].ToString() != "")
                {
                    this.cost = Convert.ToDecimal(double.Parse(jsonSingleOrderItemObject["cost"].ToString()));
                }
                else
                {
                    this.cost = 0.00M;
                }

                if (jsonSingleOrderItemObject["deleted"].ToString() != "" && jsonSingleOrderItemObject["deleted"].ToString() != null)
                {
                    this.deleted = Convert.ToBoolean(jsonSingleOrderItemObject["deleted"].ToString());
                }
                else
                {
                    this.deleted = false;
                }


                this.discount = jsonSingleOrderItemObject["discount"].ToString();


                this.ervc_type = jsonSingleOrderItemObject["ervc_type"].ToString();

                this.initial_price = Convert.ToDecimal(double.Parse(jsonSingleOrderItemObject["initial_price"].ToString()));
                this.tax_amount = Convert.ToDecimal(double.Parse(jsonSingleOrderItemObject["tax_amount"].ToString()));
                this.taxed_flag = Convert.ToBoolean(jsonSingleOrderItemObject["taxed_flag"].ToString());
                this.uuid = jsonSingleOrderItemObject["uuid"].ToString();
                this.product_name_override = jsonSingleOrderItemObject["product_name_override"].ToString();
                this.price = Convert.ToDecimal(double.Parse(jsonSingleOrderItemObject["price"].ToString()));
                this.quantity = Convert.ToInt32(jsonSingleOrderItemObject["quantity"].ToString());
                this.exchange_discount = ((jsonSingleOrderItemObject["exchange_discount"]).ToString() == "" ||
                                          (jsonSingleOrderItemObject["exchange_discount"]).ToString() == null)
                    ? null
                    : Convert.ToBoolean(jsonSingleOrderItemObject["exchanged"].ToString());


                if (jsonSingleOrderItemObject["exchanged"].ToString() != "" && jsonSingleOrderItemObject["exchanged"].ToString() != null)
                {
                    this.exchanged = Convert.ToBoolean(jsonSingleOrderItemObject["exchanged"].ToString());
                }
                else
                {
                    this.exchanged = false;
                }



                //added variables

                //tax
                this.total_price_after_tax = (this.taxed_flag == true) ? initial_price + tax_amount : initial_price;


                //parse parent orderID
                var parent_order_idParse = 0;
                if (this.order.Contains(":"))
                {
                    parent_order_idParse = RevelHelper.ConvertParentOrderIDToIntegerPrimaryKey(this.order);
                }
                else
                {
                    parent_order_idParse = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(this.order);
                }

                this.parent_order_id = parent_order_idParse;




                this.product_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(product);

                //added variables 08-07-2014

                this.created_by = jsonSingleOrderItemObject["created_by"].ToString("yyyy-MM-dd HH:mm:ss");

                if ((jsonSingleOrderItemObject["voided_date"]) == null)
                {

                }
                else
                {
                    this.voided_date = Convert.ToDateTime(jsonSingleOrderItemObject["voided_date"].ToString("yyyy-MM-dd HH:mm:ss"));
                }


                if (((string)jsonSingleOrderItemObject["discount_amount"]) == "" ||
                    ((string)jsonSingleOrderItemObject["discount_amount"]) == null)
                {
                    this.discount_amount = 0.00M;
                }
                else
                {
                    this.discount_amount = Convert.ToDecimal(double.Parse((string)jsonSingleOrderItemObject["discount_amount"]));
                }


                if (((string)jsonSingleOrderItemObject["discount_taxed"]) == "" ||
                   ((string)jsonSingleOrderItemObject["discount_taxed"]) == null)
                {
                    this.discount_taxed = false;
                }
                else
                {

                    this.discount_taxed = true;
                }



                //added 12-08-2014
                this.pure_sales = Convert.ToDecimal(double.Parse((string)jsonSingleOrderItemObject["pure_sales"]));
                this.modifier_amount = Convert.ToDecimal(double.Parse((string)jsonSingleOrderItemObject["modifier_amount"]));
                this.cost = Convert.ToDecimal(double.Parse((string)jsonSingleOrderItemObject["cost"]));
                this.modifier_cost = Convert.ToDecimal(double.Parse((string)jsonSingleOrderItemObject["modifier_cost"]));
                this.discount_reason = jsonSingleOrderItemObject["discount_reason"].ToString();
                this.resource_uri = jsonSingleOrderItemObject["resource_uri"].ToString();
                //parse the discount
                /*    if (discount_amount > 0)
                    {
                        try
                        {
                            this.discountMoneyAmountACTUAL = CalculateRealDiscountMoneyAmount();
                        }
                        catch (Exception)
                        {
                           var id = orderitem_id;

                        }
                    }*/


                try
                {
                    if ((string)jsonSingleOrderItemObject["updated_date"] != null &&
                             (string)jsonSingleOrderItemObject["updated_date"] != "")
                    {
                        updated_date = Convert.ToDateTime(jsonSingleOrderItemObject["updated_date"].ToString("yyyy-MM-dd HH:mm:ss"));
                    }


                }
                catch (Exception ex)
                {

                    throw;
                }
                try
                {
                    if ((string)jsonSingleOrderItemObject["start_time"] != null &&
                             (string)jsonSingleOrderItemObject["start_time"] != "")
                    {
                        start_time = Convert.ToDateTime(jsonSingleOrderItemObject["start_time"].ToString("yyyy-MM-dd HH:mm:ss"));
                    }


                }
                catch (Exception ex)
                {

                    throw;
                }


                try
                {
                    if ((string)jsonSingleOrderItemObject["kitchen_completed"] != null &&
                             (string)jsonSingleOrderItemObject["kitchen_completed"] != "")
                    {
                        kitchen_completed = Convert.ToDateTime(jsonSingleOrderItemObject["kitchen_completed"].ToString("yyyy-MM-dd HH:mm:ss"));
                    }


                }


                catch (Exception ex)
                {

                    throw;
                }

                try
                {
                    if ((string)jsonSingleOrderItemObject["expedited"] != null &&
                             (string)jsonSingleOrderItemObject["expedited"] != "")
                    {
                        expedited = (DateTime)jsonSingleOrderItemObject["expedited"];
                    }


                }


                catch (Exception ex)
                {

                    throw;
                }


            }
            catch (Exception exception)
            {

                var id = orderitem_id;
                throw new Exception("Couldn't Create an OrderItem", exception);
            }

            return 0;
        }

        public bool Equals(OrderItem other)
        {
            if (this.orderitem_id == other.orderitem_id) return true;

            return false;
        }

        public DateTime CreationDate
        {
            get { return (DateTime)created_date; }
        }

        public int LinkingIdToParent
        {
            get
            {
                return this.parent_order_id;

            }
        }

        public decimal Price { get { return pure_sales; } }






    }
    public static class OrderItemExtension
    {

        public static IQueryable<OrderItem> FilterCompsAndVoids(this IQueryable<OrderItem> source)
        {
            return source.Where(i => i.ervc_type != "7" || i.ervc_type != "8" || i.ervc_type != "9"
                                     || i.ervc_type != "5"
                                     || i.ervc_type != "6");

        }

    }


}