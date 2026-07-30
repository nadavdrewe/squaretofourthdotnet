namespace web.pipeline.fourth.com.Models
{
    public sealed class StaticAdminOptions
    {
        public string Username { get; set; }

        // SHA-256 hash represented as 64 hexadecimal characters.
        public string PasswordHash { get; set; }
    }
}
