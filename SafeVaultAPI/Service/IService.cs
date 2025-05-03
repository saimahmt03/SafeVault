using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.Entities;
using SafeVaultAPI.DTO.Result;

namespace SafeVaultAPI.Service
{
    public interface IService
    {
        Task<Result> AddNewUserAsync(AddNewUserRequest request);
        Task<UserList> GetAllUserAsync();
    }
}
