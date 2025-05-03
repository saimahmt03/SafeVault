namespace SafeVaultAPI.Entities
{
    public class User
    {
        public string firstname { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public int type { get; set; } = 0;
        public string applicationSignature { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
    }
}