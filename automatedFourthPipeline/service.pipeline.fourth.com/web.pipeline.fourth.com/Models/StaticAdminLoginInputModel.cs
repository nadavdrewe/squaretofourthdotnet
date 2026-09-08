using System.ComponentModel.DataAnnotations;

namespace web.pipeline.fourth.com.Models
{
    public sealed class StaticAdminLoginInputModel
    {
        [Required]
        [Display(Name = "Email or administrator username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Keep me signed in")]
        public bool RememberMe { get; set; }
    }

    public sealed class ChangePasswordInputModel
    {
        [Required, DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }

        [Required, DataType(DataType.Password), MinLength(10)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
        [Display(Name = "Confirm new password")]
        public string ConfirmPassword { get; set; }
    }
}
