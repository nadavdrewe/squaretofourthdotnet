using data.pipeline.fourth.com.Enums;
using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using shared.pipeline.fouth.com.Enums;

namespace data.pipeline.fourth.com.Models.Credentials
{

    /// <summary>
    /// To allow logging in with host system. Can be at brand level or store level
    /// </summary>
    public class BaseCredential : IAmActive, IAmCredential, IMightHaveBrandOwner, IMightHaveStoreOwner, IAmTimestampable
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        /// <summary>
        /// Oauth client Id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// OAuth client secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Oauth access token - this is the one you get first time. 
        /// </summary>
        public string LatestAccessToken { get; set; }
        /// <summary>
        /// Oauth REFRESH Token!!! This is used to log in to the API
        /// </summary>
        public string RefreshToken { get; set; } //used for oauth     
        /// <summary>
        /// Used by Revel, Lightspeed, Square this is traditional request endpoint
        /// </summary>
        public string BaseEndpoint { get; set; }        
        public string Password { get; set; }
        /// <summary>
        /// Used by Revel
        /// </summary>
        public string KeySecret { get; set; } //used for traditional static key:secret

        public string SupplimentalData1 { get; set; }
        public string SupplimentalData2 { get; set; }
        /// <summary>
        /// This is of type 'IntegrationCredentialTypes'
        /// </summary>
        public shared.pipeline.fouth.com.Enums.CredentialTypes CredentialType { get; set; } = shared.pipeline.fouth.com.Enums.CredentialTypes.None;


        [ForeignKey("Store")]
        public int? StoreId { get; set; } = null;
        /// <summary>
        /// Creds can be for a store or a brand depending on use
        /// </summary>
        public virtual Store Store { get; set; }

        [ForeignKey("Brand")]
        public int? BrandId { get; set; } = null;
        /// <summary>
        /// Creds can be for a store or a brand depending on use
        /// </summary>
        public virtual Brand Brand { get; set; }
        public bool Active { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
