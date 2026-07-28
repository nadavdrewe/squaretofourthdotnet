using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Revel._808nd.com.Classes
{
    public class RevelOrganisation
    {
        public RevelOrganisation()
        {
            
        }
        public RevelOrganisation(string orgName, string api_key, Uri baseURL)
        {
            this.BaseUri = baseURL;
            this.RevelOrganiationName = orgName;
            this.api_key = api_key;

        }

        public string RevelOrganiationName { get; set; }
        [NotMapped]
        public string api_key { get; set; }
        [NotMapped]
        public Uri BaseUri { get; set; }
  
    }
}
