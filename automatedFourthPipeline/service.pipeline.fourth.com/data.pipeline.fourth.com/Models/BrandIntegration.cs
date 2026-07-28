using data.pipeline.fourth.com.Interfaces.Public;
using data.pipeline.fourth.com.Models.CredentialTypes;
using shared.pipeline.fourth.com;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data.pipeline.fourth.com.Models
{
    public class BrandIntegration : IAmActive
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Brand")]
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        /// <summary>
        /// Value of integration type - checked by service
        /// </summary>
        public IntegrationTypes IntegrationType { get; set; } = IntegrationTypes.None;
        /// <summary>
        /// Sub type of integration (if necessary)
        /// </summary>
        public IntegrationSubTypes IntegrationSubType { get; set; } = IntegrationSubTypes.None;
        /// <summary>
        /// Is it on / off?
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// When the batch fires UTC - integer from 0 - 23
        /// </summary>
        [Range(0, 23)]
        public int? StartBatchQueryTimeUTC { get; set; }

    }
}
