using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.pipeline.fourth.com.Models.Mappings
{
    public class SquareEmployeeMapping : IAmActive, IAmTimestampable
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StoreIntegration")]
        public int StoreIntegrationId { get; set; }
        public virtual StoreIntegration StoreIntegration { get; set; }

        [Required]
        [StringLength(128)]
        public string SquareTeamMemberId { get; set; }

        [StringLength(256)]
        public string SquareDisplayName { get; set; }

        [Required]
        [StringLength(128)]
        public string FourthEmployeeNumber { get; set; }

        public bool Active { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
