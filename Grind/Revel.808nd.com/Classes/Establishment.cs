using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{
    public class Establishment : RevelOrganisation, IRevelAddressable, IRevelCreateable
    {
        [JsonIgnore]
        public int NumberOfMinutesAfterOpenThatIsLate { get; set; }
        [JsonIgnore]
        public Dictionary<int, string> bindingTable { get; set; }

        [Key]
        [JsonIgnore]
        public int DBKEY_establishment_id { get; set; }

        [JsonIgnore]
        public int establishment_id { get; set; }

        [JsonIgnore]
        public ICollection<OpeningHours> OpeningHours { get; set; }

        [JsonIgnore]
        [NotMapped]
        public Dictionary<int, int> AnnualBudget { get; set; }

        [JsonIgnore]
        public bool is_fourth_active { get; set; }

        [JsonIgnore]
        public string theAddress { get; set; }
        [JsonIgnore]
        public string fourth_locationID { get; set; }

        //REVEL Attributes
        public string address { get; set; }
        public string brand { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string resource_uri { get; set; }
        public string location_email { get; set; }
        public string time_zone { get; set; }
        public DateTime effective_from { get; set; }
        public string id { get; set; }
        public int db_brand_id { get; set; }
        public ICollection<CashupNotifier> CashupNotifiers { get; set; }
        public ICollection<Projection> Projections { get; set; }


        public Establishment(int EstablishmentID, string orgName, string api_key, Uri baseURL)
            : base(orgName, api_key, baseURL)
        {
            this.establishment_id = EstablishmentID;
            this.AssignAnnualBudget();
            this.AssignOpeningHours();
            this.theAddress = "/enterprise/Establishment/?format=json";
        }


        public Establishment()
        {
            this.theAddress = "/enterprise/Establishment/?format=json";
        }

        private bool AssignOpeningHours()
        {
            try
            {
                this.OpeningHours = new List<OpeningHours>();
                switch (this.establishment_id)
                {
                    case 1:
                        break;
                    case 3:
                        break;
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool AssignAnnualBudget()
        {
            try
            {
                this.AnnualBudget = new Dictionary<int, int>();
                switch (this.establishment_id)
                {
                    case 1:
                        AnnualBudget.Add(1, 106480);
                        AnnualBudget.Add(2, 106480);
                        AnnualBudget.Add(3, 133450);
                        AnnualBudget.Add(4, 103580);
                        AnnualBudget.Add(5, 130950);
                        AnnualBudget.Add(6, 105380);
                        AnnualBudget.Add(7, 106180);
                        AnnualBudget.Add(8, 132740);
                        AnnualBudget.Add(9, 106480);
                        AnnualBudget.Add(10, 132750);
                        AnnualBudget.Add(11, 106480);
                        AnnualBudget.Add(12, 106480);
                        break;
                    //soho
                    case 3:
                        AnnualBudget.Add(1, 35624);
                        AnnualBudget.Add(2, 35624);
                        AnnualBudget.Add(3, 44780);
                        AnnualBudget.Add(4, 35624);
                        AnnualBudget.Add(5, 44780);
                        AnnualBudget.Add(6, 35624);
                        AnnualBudget.Add(7, 35624);
                        AnnualBudget.Add(8, 44780);
                        AnnualBudget.Add(9, 35624);
                        AnnualBudget.Add(10, 44780);
                        AnnualBudget.Add(11, 35624);
                        AnnualBudget.Add(12, 35624);
                        break;
                    //london
                    case 4:
                        AnnualBudget.Add(1, 85152);
                        AnnualBudget.Add(2, 85152);
                        AnnualBudget.Add(3, 106440);
                        AnnualBudget.Add(4, 85152);
                        AnnualBudget.Add(5, 106440);
                        AnnualBudget.Add(6, 85152);
                        AnnualBudget.Add(7, 85152);
                        AnnualBudget.Add(8, 106440);
                        AnnualBudget.Add(9, 85152);
                        AnnualBudget.Add(10, 106440);
                        AnnualBudget.Add(11, 85152);
                        AnnualBudget.Add(12, 85152);
                        break;
                    //holborn
                    case 5:
                        AnnualBudget.Add(1, 53170);
                        AnnualBudget.Add(2, 53170);
                        AnnualBudget.Add(3, 66462);
                        AnnualBudget.Add(4, 48838);
                        AnnualBudget.Add(5, 53170);
                        AnnualBudget.Add(6, 66462);
                        AnnualBudget.Add(7, 53483);
                        AnnualBudget.Add(8, 53170);
                        AnnualBudget.Add(9, 66462);
                        AnnualBudget.Add(10, 53170);
                        AnnualBudget.Add(11, 53170);
                        AnnualBudget.Add(12, 67462);
                        break;

                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }





        }



        public DateTime? GetTodaysOpeningTime()
        {
            DateTime? openingTime = this.OpeningHours.Where(x => x.Day == DateTime.Now.DayOfWeek).First().OpeningTime;

            //Convert to dateTime


            return openingTime;
        }


        public static IEnumerable<Establishment> GetEstablishments()
        {
            GrindContext _db = new GrindContext();

            var ests = _db.Establishments.ToList();

            return ests;
        }




        public int Create(dynamic est)
        {
            address = est["address"];
            brand = est["brand"]; //this is NOT THE REVEL BRAND - it is used as the internal base URL in this library e.g "https://dietcoke.com/
            email = est["email"];
            resource_uri = est["resource_uri"];
            location_email = est["location_email"];
            time_zone = est["time_zone"];
            name = est["name"];

            try
            {
                this.establishment_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)est["resource_uri"]);
                this.id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)est["resource_uri"]).ToString();
                fourth_locationID = id;
            }
            catch (Exception ex1)
            {
                try
                {
                    this.establishment_id = RevelHelper.ConvertParentOrderIDToIntegerPrimaryKey((string)est["resource_uri"]);
                    this.id = RevelHelper.ConvertParentOrderIDToIntegerPrimaryKey((string)est["resource_uri"]).ToString();
                    fourth_locationID = id;
                }
                catch (Exception ex2)
                {

                    try
                    {
                        this.establishment_id = RevelHelper.ConvertSpecialistIDWithSpacesToID((string)est["resource_uri"]);
                        this.id = RevelHelper.ConvertSpecialistIDWithSpacesToID((string)est["resource_uri"]).ToString();
                        fourth_locationID = id;
                    }
                    catch (Exception ex3)
                    {
                        //stumped
                        throw ex3;
                    }
                }
            }


            try
            {
                effective_from = Convert.ToDateTime(est["effective_from"].ToString("yyyy-MM-dd hh:mm:ss"));
            }
            catch (Exception)
            {


            }

            return 0;
        }


    }

    public class OpeningHours
    {
        [Key]
        public int OpeningHoursID { get; set; }
        public DayOfWeek Day { get; set; }
        public DateTime? OpeningTime { get; set; }
        public DateTime? ClosingTime { get; set; }
        public virtual Establishment Establishment { get; set; }
    }
}

