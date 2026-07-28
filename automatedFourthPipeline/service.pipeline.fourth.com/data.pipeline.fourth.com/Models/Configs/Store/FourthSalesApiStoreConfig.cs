using com.fourth.pipeline.pos.Enum;
using com.fourth.pipeline.pos.Model;
using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace data.pipeline.fourth.com.Models.Configs.Store
{
    public class FourthSalesApiStoreConfig : IAmActive, IAmTimestampable
    {
        [Key]
        public int Id { get; set; }

        //FOURTH ATTS
        public string UnitId { get; set; }
        public string SiteLocationCode { get; set; }
        public string RevenueCenter { get; set; }

        public FourthSalesRevenueCenterMappingType RevenueCenterMappingType { get; set; } = FourthSalesRevenueCenterMappingType.None;
        /// <summary>
        /// Depending on what setup is, could be revenue center by category or by store (or something else..)
        /// </summary>
        public ICollection<RevenueCenterCategoryMapping> RevenueCenterCategoryMappings { get; set; }



        [ForeignKey("StoreIntegrationId")]
        public int StoreIntegrationId { get; set; }
        public StoreIntegration StoreIntegration { get; set; }



        public bool Active { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
