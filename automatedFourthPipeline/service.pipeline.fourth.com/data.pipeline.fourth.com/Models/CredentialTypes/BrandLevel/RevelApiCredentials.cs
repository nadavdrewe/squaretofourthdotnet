using data.pipeline.fourth.com.Enums;
using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.pipeline.fourth.com.Models.CredentialTypes.BrandLevel
{
    public class RevelApiCredentials : IAmActive, IAmCredential, IHaveBrandOwner
    {
        /// <summary>
        /// Base URL of the Revelup (with a slash afterwards)
        /// </summary>
        public string RevelupUrl { get; set; }
        public string KeySecret { get; set; }
        public int BrandId { get; set; }
        public shared.pipeline.fouth.com.Enums.CredentialTypes CredentialType { get; set; } = shared.pipeline.fouth.com.Enums.CredentialTypes.None;
        public bool Active { get; set; }
    }
}
