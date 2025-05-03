using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Entities;
using SafeVaultAPI.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;

namespace SafeVaultAPI.Repository
{
    internal class Repository : IRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly string _cacheKey = "UserListCache";  // Cache key for UserList

        public Repository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<Result> AddNewUserAsync(AddNewUserRequest request)
        {
            Result result = new Result();

            var hasher = new PasswordHasher<User>();
            var user = new User
            {   
                firstname = request.firstname,
                lastname = request.lastname,
                email = request.email,
                username = request.username,
                password = request.password,
                type = request.type,
                applicationSignature = request.applicationSignature
            };

            user.HashedPassword = hasher.HashPassword(user, request.password);

            // Get the current UserList from the cache, or create a new one if not found
            var userList = _memoryCache.Get<UserList>(_cacheKey) ?? new UserList();

            userList.Users.Add(user);

            // Store the updated UserList in the cache
            _memoryCache.Set(_cacheKey, userList);

            result.Code = BaseResult.ResultCode.Success;
            result.Message = BaseResult.ResultMessage.Success;

            return result;
        }

        public async Task<UserList> GetAllUserAsync()
        {
            // Retrieve the UserList from the cache
            var userList = _memoryCache.Get<UserList>(_cacheKey);

            return userList ?? new UserList();
        }
    }
}