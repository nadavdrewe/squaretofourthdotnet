namespace web.pipeline.fourth.com.Models
{
    public class PublicOnboardingOptions
    {
        public bool Enabled { get; set; }
    }

    public class PublicOnboardingInputModel
    {
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string SetupKey { get; set; }
    }

    public class PublicOnboardingResultViewModel
    {
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string TemporaryPassword { get; set; }
        public int BrandId { get; set; }
        public bool EmailSent { get; set; }
    }
}
