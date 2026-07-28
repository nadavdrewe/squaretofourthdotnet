namespace web.pipeline.fourth.com.Models
{
    public sealed class SquareOAuthOptions
    {
        public SquareOAuthEnvironmentOptions Sandbox { get; set; } = new SquareOAuthEnvironmentOptions();
        public SquareOAuthEnvironmentOptions Production { get; set; } = new SquareOAuthEnvironmentOptions();
    }

    public sealed class SquareOAuthEnvironmentOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
}
