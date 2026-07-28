
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace com.fourth.pipeline.pos.Services.SalesApi
{
    public class FourthApiService : BaseFourthService
    {


        public FourthApiService(string username, string password, string url) : base(username, password, url)
        {
        }


        public FourthApiService(string username, string password, string url, HttpClient client) : base(username, password, url, client)
        {
        }

        public FourthApiService(
            string username,
            string password,
            string apiBaseUrl,
            string clientId,
            string clientSecret,
            string scope,
            string tokenUrl,
            string grantType = "client_credentials")
            : base(username, password, apiBaseUrl, clientId, clientSecret, scope, tokenUrl, grantType)
        {
        }

        public FourthApiService(
            string username,
            string password,
            string apiBaseUrl,
            string clientId,
            string clientSecret,
            string scope,
            string tokenUrl,
            HttpClient client,
            string grantType = "client_credentials")
            : base(username, password, apiBaseUrl, clientId, clientSecret, scope, tokenUrl, client, grantType)
        {
        }
        /// <summary>
        /// THis posts csv to Fourth
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendSalesDataToFourth(string filename,
            string fulllPathToFile,
            string urlToSendTo = "api/Files")
        {
            if (!String.IsNullOrWhiteSpace(this.AccessToken))
            {
                var formContent = new MultipartFormDataContent("NKdKd9Yk");
                //push sales csv
                formContent.Headers.ContentType.MediaType = "multipart/form-data";
                // 3. Add the filename C:\\... + fileName is the path your file
                Stream fileStream = System.IO.File.OpenRead(fulllPathToFile);
                formContent.Add(new StreamContent(fileStream), filename, filename);

                //var newCLient = new HttpClient();
                //newCLient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                //return await _client.PostAsync(urlToSendTo, formContent);

                return await _client.PostAsync(urlToSendTo, formContent);
            }
            else
            {
                throw new Exception("This fourth slaes api client is not logged in, but you are trying to send data");
            }
        }

        public async Task<HttpResponseMessage> SendXmlDataToFourth(string xmlString, string xmlEndPoint)
        {
            if (!String.IsNullOrWhiteSpace(this.AccessToken))
            {
                var httpContent = new StringContent(xmlString, Encoding.UTF8, "application/xml");
                try
                {
                    return await _client.PostAsync(xmlEndPoint, httpContent);
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't send XML data to Fourth", ex);
                }

            }
            else
            {
                throw new Exception("This fourth slaes api client is not logged in, but you are trying to send data");
            }
        }
    }
}
