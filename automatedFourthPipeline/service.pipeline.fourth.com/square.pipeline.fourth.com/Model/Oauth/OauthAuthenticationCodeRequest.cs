using shared.pipeline.fouth.com.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace square.pipeline.fourth.com.Model.Oauth
{
    public class OauthAuthenticationCodeRequest
    {
        public int BrandId { get; set; }
        public CredentialTypes CredentialType { get; set; }
        //public string ClientId { get; set; } //also called 'Application Id'
        //public string ClientSecret { get; set; }//also called 'Application Secret'
    }
}
