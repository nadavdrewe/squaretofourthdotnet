using System;
using System.ComponentModel.DataAnnotations;
using data.pipeline.fourth.com.Interfaces.Public;

namespace data.pipeline.fourth.com.Models.Credentials
{
    public class SquareOAuthApplication : IAmActive, IAmTimestampable
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string Environment { get; set; }

        [Required, MaxLength(191)]
        public string ApplicationId { get; set; }

        [Required, MaxLength(1024)]
        public string ClientSecret { get; set; }

        [Required, MaxLength(2048)]
        public string RedirectUri { get; set; }

        public bool Active { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
