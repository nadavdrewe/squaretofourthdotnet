using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data.pipeline.fourth.com.Models.Configs.Store
{

    public class SquareStoreConfig : IAmActive, IAmTimestampable
    {
        [Key]
        public int Id { get; set; }
        public string LocationId { get; set; } //this is primary key in host system


        [ForeignKey("StoreIntegrationId")]
        public int StoreIntegrationId { get; set; }
        public StoreIntegration StoreIntegration { get; set; }

        public bool Active { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
