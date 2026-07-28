using data.pipeline.fourth.com.Enums;
using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data.pipeline.fourth.com.Models.CredentialTypes
{
    public class SquareApiCredentials : IAmActive, IAmCredential, IHaveBrandOwner
    {
        [Key]
        public int Id { get; set; }


        public shared.pipeline.fouth.com.Enums.CredentialTypes CredentialType { get; set; } = shared.pipeline.fouth.com.Enums.CredentialTypes.None;
        /// <summary>
        /// Are these the active credentials?
        /// </summary>
        public bool Active { get; set; }
        public int BrandId { get; set; }
    }
}
