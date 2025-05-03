
namespace SafeVaultAPI.DTO.Result
{
    public class LoginResult
    {
        public string Username { get; set; } = string.Empty;
        public int Role { get; set; } = 0;
        public string ApplicationName { get; set; } = string.Empty;
        public bool Status { get; set; } 
    }
}