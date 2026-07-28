using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.NewHttpClient
{
    public class NewRevelHttpClient
    {
        private readonly Uri BaseAddress;
        private readonly string DefaultHeaderAPIkey;

        public NewRevelHttpClient(string baseAddress, string apiKey)
        {
            BaseAddress = new Uri(baseAddress);
            DefaultHeaderAPIkey = apiKey;
        }

        public HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(600);
            client.BaseAddress = BaseAddress;

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("API-AUTHENTICATION", DefaultHeaderAPIkey);
            client.DefaultRequestHeaders.Add("Referer", BaseAddress.ToString());

            return client;
        }

        public async Task<string> GetAsync(string resource)
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(resource);
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> PostAsync(string resource, string jsonContent)
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(resource, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> PutAsync(string resource, string jsonContent)
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(resource, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> PatchAsync(string resource, string jsonContent)
        {
            using (HttpClient client = CreateHttpClient())
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), resource)
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = await client.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
        }


    }
}
