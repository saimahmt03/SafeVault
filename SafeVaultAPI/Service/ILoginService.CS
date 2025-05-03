using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;

namespace SafeVaultAPI.Service
{
    public interface ILoginService
    {   
        Task<LoginResult> LoginAsync(LoginRequest request);
    }
}