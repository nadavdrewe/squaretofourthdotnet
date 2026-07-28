using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using client.fourth.com.fhAPI;

namespace client.fourth.com
{
    public class FourthClient
    {
        private fhAPI.fhAPISoapClient fhAPI = new fhAPISoapClient();
        public AuthenticationHeader LoginToken { get; set; }

        public AuthenticationHeader Login(string user, string password)
        {
            try
            {
                var token = fhAPI.Login(user, password);
                this.LoginToken = token;
            }
            catch (Exception ex)
            {
                    
                throw new Exception("Fourth Client was unable to log in", ex);
            }

            return this.LoginToken;
        }

    }
}
