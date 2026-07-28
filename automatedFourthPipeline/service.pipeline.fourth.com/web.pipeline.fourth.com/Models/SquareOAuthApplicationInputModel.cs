using System.ComponentModel.DataAnnotations;

namespace web.pipeline.fourth.com.Models
{
    public sealed class SquareOAuthApplicationInputModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Application name")]
        public string Name { get; set; }

        [Required]
        public string Environment { get; set; }

        [Required, StringLength(191)]
        [Display(Name = "Square application ID")]
        public string ApplicationId { get; set; }

        [StringLength(1024)]
        [DataType(DataType.Password)]
        [Display(Name = "Square application secret")]
        public string ClientSecret { get; set; }

        [Required, StringLength(2048)]
        [Url]
        [Display(Name = "OAuth redirect URL")]
        public string RedirectUri { get; set; }

        public bool Active { get; set; } = true;
    }
}
