using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data.pipeline.fourth.com.Models.Configs.Store
{
    public class LightspeedRestoStoreConfig : IAmActive, IAmTimestampable
    {
        [Key]
        public int Id { get; set; }

        ///// <summary>
        ///// Name of location / store / venue e.g. Liverpool Street or Greenwich
        ///// </summary>
        //public string NameOnHostSystem { get; set; }
        ///// <summary>
        ///// This is the url of the object on the host system e.g. '/establihsment/1' in Revel
        ///// </summary>
        //public string ResourceUriOnHostSystem { get; set; }
        ///// <summary>
        ///// The primary key on the host system - e.g establishment id in Revel or CompanyId in LS Resto
        ///// </summary>
        //public string PrimaryKeyOnHostSystem { get; set; }      

        [ForeignKey("StoreIntegrationId")]
        public int StoreIntegrationId { get; set; }
        public StoreIntegration StoreIntegration { get; set; }
        public int LsRestoCompanyId { get; set; }
        public string Currency { get; set; }
        public bool Active { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
