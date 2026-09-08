using System;
using System.ComponentModel.DataAnnotations;

namespace data.pipeline.fourth.com.Models
{
    public class CustomerOnboardingInvite
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string CustomerName { get; set; }
        [MaxLength(320)]
        public string Email { get; set; }
        [Required, MaxLength(64)]
        public string KeyHash { get; set; }
        public bool Active { get; set; } = true;
        public int MaxUses { get; set; } = 1;
        public int UseCount { get; set; }
        public DateTime? ExpiresAtUTC { get; set; }
        public DateTime? LastUsedUTC { get; set; }
        public int? RedeemedBrandId { get; set; }
        [MaxLength(450)]
        public string RedeemedUserId { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
