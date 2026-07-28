using System.ComponentModel.DataAnnotations;

namespace web.pipeline.fourth.com.Models
{
    public sealed class StaticAdminLoginInputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Keep me signed in")]
        public bool RememberMe { get; set; }
    }
}
