using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    public class RewardsCardNew : IRevelAddressable, IRevelCreateable, IRevelDeletable, IExpire, IIdentifiable, IPrimaryKeyable
    {


        [JsonIgnore]
        public string ResourceUri
        {
            get { return resource_uri; }

            set { resource_uri = value; }
        }


        [JsonIgnore]
        [Key]
        public int DBKEY_rewardscardnew_id { get; set; }
        /*public string address { get; set; }*/
        public string created_by { get; set; }

        /*       [Column(TypeName = "DateTime2")]*/

        public DateTime created_date { get; set; }
        public int current_points { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public string customer_revel { get; set; }
        public string establishment { get; set; }
        [JsonProperty("id")]
        public int Revelid { get; set; }
        public string number { get; set; }
        public int payment_type { get; set; }
        public string resource_uri { get; set; }
        public int total_points { get; set; }
        public decimal total_purchases { get; set; }
        public int total_visits { get; set; }
        public string updated_by { get; set; }

        /*        [Column(TypeName = "DateTime2")]*/
        public DateTime updated_date { get; set; }

        //added be ME
        [JsonIgnore]
        public int customer_id { get; set; }
        [JsonIgnore]
        public int establishment_id { get; set; }
        [JsonIgnore]
        public bool? is_vip_card { get; set; }
        [JsonIgnore]
        public int vip_points_refresh { get; set; }

        /*        [Column(TypeName = "DateTime2")]*/
        [JsonIgnore]
        public DateTime vip_points_last_refreshed { get; set; }

        [JsonIgnore]
        [NotMapped]
        public Customer Customer { get; set; }

        [JsonIgnore]
        public string notes { get; set; }

        [JsonIgnore]
        public Nullable<int> days_since_last_visit { get; set; }

        [JsonIgnore]
        public Nullable<int> yesterdaysTotalPoints { get; set; }

        /*        [Column(TypeName = "DateTime2")]*/
        [JsonIgnore]
        public Nullable<DateTime> yesterdaysTotalPointsWhenCreated { get; set; }

        /* [Column(TypeName = "DateTime2")]*/
        [JsonIgnore]
        public Nullable<DateTime> pointsMultiplierLastRun { get; set; }



        [JsonIgnore]
        public virtual LoyaltyCardType LoyaltyCardType { get; set; }
        [JsonIgnore]
        public DateTime? ExpiryDate { get; set; }
        [JsonIgnore]
        public bool? Active { get; set; }

        /// <summary>
        /// Comma seperated list of stores 
        /// </summary>
        [JsonIgnore]
        public string StoresVisted { get; set; }
        [JsonIgnore]
        [NotMapped]
        public string Identifier { get { return number; } }




        public IList<string> GetStoresVisted()
        {
            if (!String.IsNullOrEmpty(StoresVisted))
            {
                var stores = StoresVisted.Split(',');
                return stores.Select(store => store.ToLower().Trim()).ToList();
            }

            return new List<string>();
        }

        public void AddNewStoreVisited(string newStored)
        {
            newStored = newStored.ToLower().Trim();
            StoresVisted += newStored + ",";
        }


        public RewardsCardNew()
        {
            theAddress = @"/resources/RewardsCardNew?format=json&id__gt={0}";
        }

        public RewardsCardNew(string url)
        {
            theAddress = url;
        }


        public int Create(dynamic jsonLoyaltyCardNew)
        {


            try
            {

                Revelid = jsonLoyaltyCardNew["id"];
                if (Revelid == 1000)
                {
                    var whatprobelem = true;
                }

                created_by = jsonLoyaltyCardNew["created_by"];

                if (jsonLoyaltyCardNew["created_date"].ToString() != "" && jsonLoyaltyCardNew["created_date"].ToString() != null)
                {
                    created_date = Convert.ToDateTime(jsonLoyaltyCardNew["created_date"].ToString("yyyy-MM-dd HH:mm:ss"));
                }

                current_points = Convert.ToInt32(jsonLoyaltyCardNew["current_points"].ToString());
                customer_revel = jsonLoyaltyCardNew["customer"];
                establishment = jsonLoyaltyCardNew["establishment"];

                number = jsonLoyaltyCardNew["number"];
                payment_type = jsonLoyaltyCardNew["payment_type"];
                resource_uri = jsonLoyaltyCardNew["resource_uri"];
                total_points = Convert.ToInt32(jsonLoyaltyCardNew["total_points"].ToString());
                total_purchases = Convert.ToDecimal(jsonLoyaltyCardNew["total_purchases"].ToString());
                total_visits = Convert.ToInt32(jsonLoyaltyCardNew["total_visits"].ToString());
                updated_by = jsonLoyaltyCardNew["updated_by"];
                ResourceUri = resource_uri;
                try
                {
                    if (jsonLoyaltyCardNew["updated_date"].ToString() != "" && jsonLoyaltyCardNew["updated_date"].ToString() != null)
                    {
                        updated_date = Convert.ToDateTime(jsonLoyaltyCardNew["updated_date"].ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                catch (Exception ex)
                {


                }


                string testIfCharExistsInEstID = (string)jsonLoyaltyCardNew["establishment"].ToString();

                if (testIfCharExistsInEstID != null && testIfCharExistsInEstID != "")
                {

                    try
                    {
                        this.establishment_id =
                           RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                               (string)jsonLoyaltyCardNew["establishment"]);

                    }
                    catch (Exception)
                    {
                        this.establishment_id = 0;
                    }

                }





                //if there's a customer_revel
                if (customer_revel != null)
                {
                    try
                    {
                        this.customer_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)jsonLoyaltyCardNew["customer_revel"]);
                    }
                    catch (Exception)
                    {

                        this.customer_id = 0;
                    }
                }
            }
            catch (Exception ex)
            {

                var fail = jsonLoyaltyCardNew;
            }

            return 0;
        }

        [JsonIgnore]
        public string theAddress { get; set; }



        public static List<RewardsCardNew> GetRewardCardsNewAndCustomerAsNoTracking(GrindContext db)
        {
            List<RewardsCardNew> cards = new List<RewardsCardNew>();
                                
            cards = db.RewardsCardNew.Where(x => x.number != null).Include(x => x.LoyaltyCardType).AsNoTracking().ToList();
            var customers = db.Customers.AsNoTracking().ToList();

            Customer cus = new Customer();
            foreach (var card in cards)
            {
                card.Customer = FindCustomerForCard(card, customers);
            }



            return cards.OrderByDescending(x => x.Revelid).ToList();

        }


        public static List<RewardsCardNew> GetRewardCardsNewAndCustomer()
        {
            List<RewardsCardNew> cards = new List<RewardsCardNew>();

            using (GrindContext db = new GrindContext())
            {
                cards = db.RewardsCardNew.Where(x => x.number != null).Include(x => x.LoyaltyCardType).ToList();
                var customers = db.Customers.ToList();



                Customer cus = new Customer();

                foreach (var card in cards)
                {

                    card.Customer = FindCustomerForCard(card, customers);
                }
            }


            return cards.OrderByDescending(x => x.Revelid).ToList();
        }

        public static Customer FindCustomerForCard(RewardsCardNew card, List<Customer> customers)
        {
            var cus = new Customer();
            try
            {
                if (card.number != null)
                {
                    var customersWithLic = customers.Where(x => x.LicNumber != null).ToList();

                    cus =
                        customersWithLic.FirstOrDefault(x => x.LicNumber.Trim() == card.number.Trim());
                }
                if (cus == null && card.customer_revel != null) // try get it via the Revel link
                {
                    var customersWithURIs = customers.Where(x => x.ResourceUri != null).ToList();

                    cus =
                        customersWithURIs.FirstOrDefault(x => x.ResourceUri.Trim() == card.customer_revel.Trim());
                }

                if (cus == null) // doesn't exist, new one up
                {
                    cus = new Customer
                    {
                        FirstName = "",
                        LastName = "",
                        Email = "",
                        Active = false,
                    };
                }
                else
                {
                    //stop for debuggin
                    var customerForCard = cus;
                }


                if (cus.FirstName == null)
                {
                    cus.FirstName = "";
                }
                if (cus.LastName == null)
                {
                    cus.LastName = "";
                }
                if (cus.Email == null)
                {
                    cus.Email = "";
                }
            }
            catch (Exception exception)
            {
                throw new Exception("An issue has occured in the RewardsCardNew customer identification service", exception);
            }
            return cus;
        }


        public int PrimaryKey { get { return DBKEY_rewardscardnew_id; } }
    }





}

