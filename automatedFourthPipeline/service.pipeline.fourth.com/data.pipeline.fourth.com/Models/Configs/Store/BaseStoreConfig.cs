using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data.pipeline.fourth.com.Models.Configs
{

    /// <summary>
    /// This is a 'store configuration object. It's the essential details of the 'store' or 'establishment' needed 
    /// </summary>
    public class BaseStoreConfig : IAmActive, IAmTimestampable
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



        //[ForeignKey("StoreIntegrationId")]
        //public int StoreIntegrationId { get; set; }
        public virtual StoreIntegration StoreIntegration { get; set; }

        /// <summary>
        /// This is the TIME in UTC time that the query should fire
        /// </summary>
        public TimeSpan StartDataQueryTimeUTC { get; set; }

        public bool Active { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
