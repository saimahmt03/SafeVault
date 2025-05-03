
namespace SafeVaultAPI.DTO.Result
{
    public class TokenResult
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string IssueBy { get; set; } = string.Empty;
        public int Duration { get; set; } = 0; 
    }
}