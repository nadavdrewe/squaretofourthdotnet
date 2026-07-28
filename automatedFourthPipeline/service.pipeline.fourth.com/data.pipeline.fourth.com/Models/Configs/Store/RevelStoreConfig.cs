using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace data.pipeline.fourth.com.Models.Configs.Store
{
    /// <summary>
    /// Mapping sets for Categories, anything else custom to the integration goes heres
    /// </summary>
    public class RevelStoreConfig : IAmActive, IAmTimestampable
    {
        [Key]
        public int Id { get; set; }


        public string EstablishmentID { get; set; }
        public string EstablishmentResourceUri { get; set; }

        [ForeignKey("StoreIntegrationId")]
        public int StoreIntegrationId { get; set; }
        public StoreIntegration StoreIntegration { get; set; }


        public bool Active { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
