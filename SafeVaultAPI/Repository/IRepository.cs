using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Entities;

namespace SafeVaultAPI.Repository
{
    public interface IRepository
    {
        Task<Result> AddNewUserAsync(AddNewUserRequest request);
        Task<UserList> GetAllUserAsync();
    }
}