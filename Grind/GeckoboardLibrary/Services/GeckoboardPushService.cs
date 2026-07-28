using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using Newtonsoft.Json;
using System.Net;
using System.IO;


namespace GeckoboardLibrary.Services
{

    /// <summary>
    /// Services takes an GeckoboardObject, checks vlaid
    /// </summary>
    public class GeckoboardPushService : IGeckoboardPushService
    {
        public GeckoboardPushService()
        {
        }

        public static string ConvertToSingleFieldTextWidgetJSON(string APIKey, string text1)
        {
            string JSONToReturn = "{\"api_key\":\"" + APIKey + "\",\"pushURL\":null,\"data\":" + "{\"item\":" + "[" + "{\"text\":\"" + text1 + "\",\"type\":0}" +

                                  "]}}";

            return JSONToReturn;
        }


        public async Task<bool> Push(GeckoboardObject aGeckoboardObject)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    //WidgetToJSON(aGeckoboardObject);
                    httpClient.BaseAddress = new Uri(aGeckoboardObject.GetPushURL());
                    string JsonToPush = WidgetToJSON(aGeckoboardObject);
                    
                                       
                    //fire and forget
                    var response = await httpClient.PostAsync("", new StringContent(JsonToPush, Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode)
                    {
                        response = await httpClient.PostAsync("", new StringContent(JsonToPush, Encoding.UTF8, "application/json"));
                    }

                    if(response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    
                    
                }

                return false;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }


        public async Task<bool> Push(GeckoboardObject aGeckoboardObject, string JSON)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(aGeckoboardObject.GetPushURL());
                    string JsonToPush = JSON;
                    //fire and forget
                    var response = await httpClient.PostAsync("", new StringContent(JsonToPush, Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode)
                    {
                        response = await httpClient.PostAsync("", new StringContent(JsonToPush, Encoding.UTF8, "application/json"));
                    }

                    return true;

                }



                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public string WidgetToJSON(GeckoboardObject aGeckoboardObject)
        {
            try
            {
                string JSONToReturn = JsonConvert.SerializeObject(aGeckoboardObject);


                return JSONToReturn;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }



    }
}
