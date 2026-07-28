using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{
    public class WebserviceDataWriter
    {
        private HttpClient client { get; set; }
        private RevelFactory factory { get; set; }
        private Establishment est { get; set; }

        private RevelWebserviceDataReader webReader { get; set; }

        private GrindContext _db { get; set; }

        public WebserviceDataWriter(Establishment est, GrindContext db)
        {

            _db = db;
            factory = new RevelFactory(est);
            client = factory.CreateHttpClient();
            webReader = new RevelWebserviceDataReader(est);
            ;

        }


        public async Task<int> DeleteRevelItem<T>(T thingToSend) where T : IRevelDeletable
        {
            var ok = await this.Delete(thingToSend, thingToSend.ResourceUri);



            return 0;
        }

        public async Task<int> CreateCustomer(Customer cus)
        {
            cus.Uuid = "";
            cus.ResourceUri = "";


            var id = await this.Create(cus, "/resources/Customer/");

            if (id > 0)
            {
                cus.RevelId = id;
                cus.ResourceUri = "/resources/Customer/" + id.ToString() + "/";
                return 0;
            }



            return -1;
        }

        public async Task<int> CreateAddress(Address address)
        {
            try
            {
                address.resource_uri = "";
                var ok = await this.Create(address, "/resources/Address/");



                throw new NotImplementedException();

                return 0;

            }
            catch (Exception)
            {

                return -1;
            }
        }


        public async Task<int> CreateGiftCard(GiftCard card)
        {
            try
            {
                card.resource_uri = "";
                var id = await this.Create(card, "/resources/GiftCard/");

                card.id = id;
                card.resource_uri = "/resources/GiftCard/" + id.ToString() + "/";
                return 0;

            }
            catch (Exception)
            {

                return -1;
            }

        }


        public async Task<int> UpdateGiftCard(GiftCard card)
        {

            var CardJSON = await this.Update(card, card.resource_uri);

            //do we need to do anything with the returned card? 

            _db.GiftCards.Attach(card);
            _db.Entry(card).State = EntityState.Modified;
            _db.SaveChanges();

            return 0;
        }


        public async Task<int> CreateRewardCard(RewardsCardNew card)
        {

            try
            {
                card.resource_uri = "";
                var id = await this.Create(card, "/resources/RewardsCardNew/");

                if (id > 0)
                {
                    card.Revelid = id;
                    card.resource_uri = "/resources/RewardsCardNew/" + id.ToString() + "/";

                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {

                return -1;
            }


        }

        public async Task<int> UpdateCustomer(Customer cus)
        {
            var customerJSON = await this.Update(cus, cus.ResourceUri);

            var returnCus = new Customer(customerJSON);

            _db.Customers.Attach(cus);
            _db.Entry(cus).State = EntityState.Modified;
            _db.SaveChanges();


            return 0;
        }

        public async Task<int> UpdateRewardCard(RewardsCardNew card)
        {

            try
            {
                var returnRewardCardJSON = await this.Update(card, card.resource_uri);
                var returnRewardCard = new RewardsCardNew(returnRewardCardJSON);

                _db.RewardsCardNew.Attach(card);
                _db.Entry(card).State = EntityState.Modified;
                _db.SaveChanges();

                return 0;
            }
            catch (Exception ex)
            {

                return -1;
            }
        }

        public async Task<int> UpdateAddress(Address address)
        {




            throw new NotImplementedException();
        }



        private async Task<int> Create<T>(T thingToSend, string placeToSendTo)
        {
            //serialise and pump up to revel
            //ensure necessary fields are blank

            //string jsonPreFormat ="{\"active\": true, \"address\": \"221 Baker Street\", \"addresses\": [], \"birth_date\": null, \"cc_exp\": null, \"cc_first_name\": null, \"cc_last_4_digits\": null, \"cc_last_name\": null, \"cc_number\": \"\", \"city\": \"London\", \"created_by\": \"/enterprise/User/14/\", \"created_date\": \"2014-03-27T06:17:43.916409\", \"customer_groups\": [], \"email\": \"test@revel.com\", \"exp_date\": null, \"first_name\": \"BANGO\", \"is_visitor\": false, \"last_name\": \"BANGO\", \"lic_number\": null, \"notes\": \"\", \"phone_number\": \"1234567890\", \"picture\": \"\", \"ref_number\": \"\", \"state\": null, \"total_purchases\": 0, \"total_visits\": 0, \"updated_by\": \"/enterprise/User/14/\", \"updated_date\": \"2015-01-01T06:17:43.916409\", \"uuid\": \"\", \"zipcode\": null}";

            string json = JsonConvert.SerializeObject(thingToSend);
            JObject jArr = JObject.Parse(json);

            jArr.Descendants().OfType<JProperty>()
                  .Where(p => p.Name == "id")
                  .ToList()
                  .ForEach(att => att.Remove());

            /*    jArr.Descendants().OfType<JProperty>()
                     .Where(p => p.Name == "resource_uri")
                     .ToList()
                     .ForEach(att => att.Remove());*/

            var newJson = jArr.ToString();

            var content = new StringContent(newJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(placeToSendTo, content);

            //
            if (response.IsSuccessStatusCode)
            {
                string location = response.Headers.GetValues("Location").FirstOrDefault();
                var array = location.Split('/');
                var id = array[3];
                string resultContent = response.Content.ReadAsStringAsync().Result;
                return Convert.ToInt32(id);

                //do we need to assign any IDs to localDB
                //log transaction success

            }
            else
            {
                //log transaction fail
                return 0;
            }

        }


        public async Task<string> BulkUpdate<T>(List<T> thingsToSend, string placeToSendTo)
        {

            var objects = new ObjectsJsonWrapper<T>();

            objects.objects = thingsToSend;


            string json = JsonConvert.SerializeObject(objects);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync(placeToSendTo, content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {

                return resultContent;
                //do we need to assign any IDs to localDB
                //log transaction success
            }
            else
            {
                //log transaction fail
                return "";
            }

            return resultContent;

        }




        public async Task<string> Update<T>(T thingToSend, string placeToSendTo)
        {


            string json = JsonConvert.SerializeObject(thingToSend);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(placeToSendTo, content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {

                return resultContent;
                //do we need to assign any IDs to localDB
                //log transaction success
            }
            else
            {
                //log transaction fail
                return "";
            }

            return resultContent;
        }



        private async Task<int> Delete<T>(T thingToSend, string placeToSendTo)
        {

            var response = await client.DeleteAsync(placeToSendTo);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            //
            if (response.IsSuccessStatusCode)
            {
                //do we need to assign any IDs to localDB
                //log transaction success
            }
            else
            {
                return -1;
            }

            return 0;
        }


    }







}
