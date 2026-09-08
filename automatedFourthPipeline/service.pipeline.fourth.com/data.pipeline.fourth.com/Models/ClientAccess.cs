using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.pipeline.fourth.com.Models
{
    public class ClientAccess
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Brand))]
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        [Required, MaxLength(450)]
        public string UserId { get; set; }
        [Required, MaxLength(320)]
        public string Email { get; set; }
        public bool IsOwner { get; set; }
        public bool Active { get; set; } = true;
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
