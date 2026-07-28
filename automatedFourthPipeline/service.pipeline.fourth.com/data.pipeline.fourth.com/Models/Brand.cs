using data.pipeline.fourth.com.Enums;
using data.pipeline.fourth.com.Interfaces.Public;
using data.pipeline.fourth.com.Models.Credentials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data.pipeline.fourth.com.Models
{
    public class Brand : IAmActive, IAmTimestampable
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public bool Active { get; set; }
        public Regions Region { get; set; } = Regions.None;
        /// <summary>
        /// E.g USD - currency type
        /// </summary>
        public CurrencyType Currency { get; set; } = CurrencyType.None;
        public shared.pipeline.fouth.com.Enums.CredentialTypes InitalSetupSytemType { get; set; }

        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<BaseCredential> BrandCredentials { get; set; }
        public virtual ICollection<BrandIntegration> BrandIntegrations { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
