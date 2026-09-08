using System;
using System.ComponentModel.DataAnnotations;

namespace web.pipeline.fourth.com.Models
{
    public class CustomerOnboardingInviteInputModel
    {
        [MaxLength(200)]
        [Display(Name = "Invite label (optional)")]
        public string CustomerName { get; set; }
        [MinLength(12), MaxLength(200)]
        [Display(Name = "Customer phrase (optional)")]
        public string Phrase { get; set; }
        [EmailAddress, MaxLength(320)]
        [Display(Name = "Restricted email (optional)")]
        public string Email { get; set; }
        [Range(1, 20)]
        [Display(Name = "Maximum uses")]
        public int MaxUses { get; set; } = 1;
        [Display(Name = "Expires at (UTC)")]
        public DateTime? ExpiresAtUTC { get; set; }
    }

    public class CustomerOnboardingInviteCreatedViewModel
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phrase { get; set; }
        public DateTime? ExpiresAtUTC { get; set; }
        public int MaxUses { get; set; }
    }
}
