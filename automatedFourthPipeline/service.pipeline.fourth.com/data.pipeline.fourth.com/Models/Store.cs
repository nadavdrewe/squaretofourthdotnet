using data.pipeline.fourth.com.Enums;
using data.pipeline.fourth.com.Interfaces.Public;
using data.pipeline.fourth.com.Models.Configs;
using data.pipeline.fourth.com.Models.Credentials;
using data.pipeline.fourth.com.Models.CredentialTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data.pipeline.fourth.com.Models
{
    public class Store : IAmActive
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// This is so we can change the orders to 'local time' for the store if necessary
        /// </summary>
        public string Timezone { get; set; }   
        /// <summary>
        /// How many hours + or - it is from UTC time - used to query data
        /// </summary>
        public int UTCOffsetInHours { get; set; }
        public bool Active { get; set; }

        [ForeignKey("Brand")]
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual ICollection<BaseCredential> StoreCredentials { get; set; }
        public virtual ICollection<StoreIntegration> StoreIntegrations { get; set; }
    }
}
