using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;


namespace SafeVaultAPI.Repository
{
    public interface ILoginRepository
    {
        Task<LoginResult> LoginAsync(LoginRequest request);
    }
}