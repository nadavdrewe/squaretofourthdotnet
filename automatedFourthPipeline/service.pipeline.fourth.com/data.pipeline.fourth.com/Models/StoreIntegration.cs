using data.pipeline.fourth.com.Interfaces.Public;
using data.pipeline.fourth.com.Models.Configs.Store;
using data.pipeline.fourth.com.Models.Mappings;
using shared.pipeline.fourth.com;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.pipeline.fourth.com.Models
{
    /// <summary>
    /// Shows active integrations of type for a particular store
    /// </summary>
    public class StoreIntegration : IAmTimestampable, IAmActive, IHaveStoreOwner
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Is it on / off?
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// String value of integration type - checked by service
        /// </summary>
        public IntegrationTypes IntegrationType { get; set; } = IntegrationTypes.None;

        /// <summary>
        /// Sub type of integration (if necessary)
        /// </summary>
        public IntegrationSubTypes IntegrationSubType { get; set; } = IntegrationSubTypes.None;

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime WhenCreatedUTC { get; set; }

        /// <summary>
        /// Last updated
        /// </summary>
        public DateTime WhenUpdatedUTC { get; set; }

        /// <summary>
        /// These are the various configs for different possible integrations
        /// </summary>
        public virtual ICollection<RevelStoreConfig> RevelStoreConfigs { get; set; }
        public virtual ICollection<FourthSalesApiStoreConfig> FourthSalesApiStoreConfigs { get; set; }
        public virtual ICollection<LightspeedRestoStoreConfig> LightspeedRestoStoreConfig { get; set; }
        public virtual ICollection<SquareStoreConfig> SquareStoreConfigs { get; set; }
        public virtual ICollection<SquareEmployeeMapping> SquareEmployeeMappings { get; set; }

        public virtual Store Store { get; set; }

        [ForeignKey("Store")]
        public int StoreId { get; set; }
    }
}
