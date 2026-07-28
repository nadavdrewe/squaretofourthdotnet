using data.pipeline.fourth.com.Enums;
using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using shared.pipeline.fouth.com.Enums;

namespace data.pipeline.fourth.com.Models.CredentialTypes
{
    public class FourthSalesApiCredentials : IAmActive, IAmCredential, IHaveBrandOwner
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string URL { get; set; }

        /// <summary>
        /// Which system type is it
        /// </summary>
        public shared.pipeline.fouth.com.Enums.CredentialTypes CredentialType { get; set; } = shared.pipeline.fouth.com.Enums.CredentialTypes.None;
        /// <summary>
        /// This links to the brand in main domain context
        /// </summary>
        public int BrandId { get; set; }
        public bool Active { get; set; }
    }
}
