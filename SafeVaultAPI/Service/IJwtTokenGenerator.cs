using SafeVaultAPI.DTO.Result;

namespace SafeVaultAPI.Service
{
    public interface IJwtTokenGenerator
    {
        TokenResult GenerateToken(string username, int role, string appsignatory);
        bool TokenValidator(string token);
    } 
}