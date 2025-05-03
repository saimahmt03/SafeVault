namespace SafeVaultAPI.Utilities
{
    public class ClientSettings
    {
        public string Client { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = "SafeVaultAPI";
        public string Audience { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
    }
}